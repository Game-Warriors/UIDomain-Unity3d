using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameWarriors.UIDomain.Data;
using GameWarriors.UIDomain.Abstraction;
using UnityEngine;
using UnityEngine.UI;

namespace GameWarriors.UIDomain.Core
{
    public class UISystem : IAspectRatio, IToast, IScreen
    {
        private readonly IUIEventHandler _uiEventHandler;
        private Dictionary<string, UIScreenItem> _screenPool;
        private List<UIScreenItemData> _screenItems;
        private List<UIScreenItem> _screenStack;
        private IToastItem[] _toastPool;
        private ImageBlackPanel _screenBackPanel;
        private Transform _lockPanel;
        private RectTransform _screenCanvasTransform;
        private Canvas _screenCanvas;
        private RectTransform _mainCanvasTransform;
        private int _positionIndex;
        private bool _backLockState;

        //public float BaseAspect => 0.5629406f;
        public float BaseAspect => 0.56240487f;
        public float CurrentAspect => (float)Screen.width / (float)Screen.height;
        public float TopSideSafeAreaOffset => (Screen.height - Screen.safeArea.height) / 2;
        public IUIScreen LastScreen => _screenStack.Count > 0 ? _screenStack[_screenStack.Count - 1] as IUIScreen : null;
        public int OpenScreenCount => _screenStack.Count;


        [UnityEngine.Scripting.Preserve]
        public UISystem(IUIEventHandler eventHandler, IUIResources resources)
        {
            _uiEventHandler = eventHandler;
            if (resources == null)
                resources = new DefaultResourseLoader();
            _uiEventHandler.SetUIUpdate(UIUpdate);
            resources.LoadResourceAsync(UIMainConfig.RESOURCES_PATH, LoadCompelete);
        }

        [UnityEngine.Scripting.Preserve]
        public async Task WaitForLoading()
        {
            while (_screenPool == null)
            {
                await Task.Delay(100);
            }
        }

        public void ShowStaticToast(string context)
        {
            IToastItem toastItem = _toastPool[0];
            toastItem.SetData(context);
            _uiEventHandler.OnToastRises(1.5f, toastItem);
        }

        public void ShowDynamicToast(string context, MessagePopUpPlace typeMessage)
        {
            IToastItem toastItem = _toastPool[_positionIndex];
            toastItem.SetData(context);
            if (typeMessage == MessagePopUpPlace.Up)
                toastItem.Position = new Vector2(0, 550 + (_positionIndex * 70));
            else
                if (typeMessage == MessagePopUpPlace.Mid)
                toastItem.Position = new Vector2(0, (_positionIndex * 70));
            else
                toastItem.Position = new Vector2(0, -550 + (_positionIndex * 70));
            ++_positionIndex;
            _positionIndex %= _toastPool.Length;
            _uiEventHandler.OnToastRises(1f, toastItem);
        }

        void IScreen.SetBackLockState(bool state)
        {
            _backLockState = state;
        }

        void IScreen.ShowScreen(string screenName, ECanvasType canvasType, EPreviosScreenAct previosScreenAct)
        {
            ShowScreen<UIScreenItem>(screenName, canvasType, previosScreenAct);
        }

        public T ShowScreen<T>(string screenName, ECanvasType canvasType, EPreviosScreenAct previosScreenAct) where T : UIScreenItem
        {
            if (string.IsNullOrEmpty(screenName))
                return default;
            Action onClose = null;
            RectTransform parent = canvasType == ECanvasType.MainCanvas ? _mainCanvasTransform : _screenCanvasTransform;
            CheckScreenInstatiateState(screenName, parent);

            UIScreenItem element = _screenPool[screenName];
            if (element.HasBlackScreen)
                _screenBackPanel.ShowIn(0.15f);
            else
                _screenBackPanel.Activation = false;

            bool isOpen = IsScreenInList(screenName);
            if (isOpen)
            {
                element = CreateScreen(screenName, parent);
                element.IsDestroy = true;
            }

            SetPrevoisScreenState(previosScreenAct, element);
            element.transform.localScale = Vector3.one;
            _screenStack.Add(element);
            element.OnShow(onClose);
            _uiEventHandler.OnOpenScreen(element);
            return element as T;
        }

        public void CloseScreen(string screenName, bool showOpenAnimation = false)
        {
            if (string.IsNullOrEmpty(screenName))
                return;

            for (int i = _screenStack.Count - 1; i > -1; --i)
            {
                if (string.Compare(_screenStack[i].ScreenName, screenName) == 0)
                {
                    UIScreenItem closeElement = _screenStack[i];
                    _screenStack.RemoveAt(i);
                    closeElement.OnClose();
                    _uiEventHandler.OnCloseScreen(closeElement);

                    UIScreenItem openScreen = null;
                    if (_screenStack.Count > 0)
                        openScreen = _screenStack[_screenStack.Count - 1];
                    else if (closeElement.HasBlackScreen)
                        _screenBackPanel.DisableScreen();

                    if (openScreen != null)
                    {
                        if (!openScreen.HasBlackScreen)
                        {
                            float length = closeElement.CloseAnimationDuration;
                            if (length > 0)
                                _screenBackPanel.FadeOut(length, null);
                            else
                                _screenBackPanel.DisableScreen();
                        }
                        else
                            if (!_screenBackPanel.Activation)
                            _screenBackPanel.ShowIn(0.15f);

                        openScreen.OnShow(showAnimation: showOpenAnimation);
                        _uiEventHandler.OnShowScreen(openScreen);
                    }
                }
            }
        }

        public void ClearScreens(bool isCloseLastScreen = true)
        {
            int length = _screenStack.Count;
            if (length < 1)
                return;
            UIScreenItem mainMenuScreen = _screenStack[0];
            int startIndex = isCloseLastScreen ? 0 : 1;
            for (int i = startIndex; i < length; ++i)
            {
                _screenStack[i].ForceExit();
                _uiEventHandler.OnScreenForceClose(_screenStack[i]);
            }
            _screenStack.Clear();
            if (startIndex == 1)
            {
                _screenStack.Add(mainMenuScreen);
                mainMenuScreen.OnShow(showAnimation: false);
                _uiEventHandler.OnShowScreen(mainMenuScreen);
            }
        }

        public bool IsScreenInList(string targetName)
        {
            if (_screenStack.Count == 0)
                return false;
            int length = _screenStack.Count;
            var result = Parallel.For(0, length, (index, state) =>
            {
                if (string.Compare(_screenStack[index].ScreenName, targetName) == 0)
                    state.Break();
            });
            return result.LowestBreakIteration != null;
        }

        void IAspectRatio.AdjustAspect(GameObject gameObject)
        {
            float cameraAspect = (float)Screen.width / Screen.height;
            if (BaseAspect > cameraAspect)
            {
                var fitter = gameObject.GetComponent<AspectRatioFitter>();
                float fitterValue = CalculateFitterValue(fitter.aspectRatio, cameraAspect);
                fitter.aspectRatio = fitterValue;
            }
        }

        public Transform ShowBlackScreen(float length)
        {
            _screenBackPanel.ShowIn(length);
            return _screenBackPanel.TransformRef;
        }

        public void HideBlackScreen(float length, Action hideBlackPanelDone)
        {
            _screenBackPanel.FadeOut(length, hideBlackPanelDone);
        }

        private void UIUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !_backLockState)
            {
                int lastIndex = _screenStack.Count - 1;
                if (lastIndex > 0)
                {
                    UIScreenItem lastScreen = _screenStack[lastIndex];
                    lastScreen.OnRequestCloseScreen(false);
                }
                else if (lastIndex == 0)
                {
                    UIScreenItem lastScreen = _screenStack[lastIndex];
                    lastScreen.OnRequestCloseScreen(true);
                    _uiEventHandler.OnCloseLastScreen();
                }
            }
        }

        private void SetPrevoisScreenState(EPreviosScreenAct previosScreenAct, UIScreenItem newScreen, float delay = 0)
        {
            if (_screenStack.Count > 0)
            {
                int index = _screenStack.Count - 1;
                UIScreenItem oldScreen = _screenStack[index];
                switch (previosScreenAct)
                {
                    case EPreviosScreenAct.Close:
                        {
                            if (index > 0)
                            {
                                _screenStack.RemoveAt(index);
                                float openAnimationDuration;
                                if (delay > 0)
                                    openAnimationDuration = delay;
                                else
                                    openAnimationDuration = oldScreen.HasBlackScreen && !newScreen.HasBlackScreen ? 0 : oldScreen.OpenAnimationDuration;
                                oldScreen.OnClose(openAnimationDuration);
                                _uiEventHandler.OnCloseScreen(oldScreen);
                            }
                            break;
                        }
                    case EPreviosScreenAct.Queue:
                        {
                            bool deactivate = oldScreen.HasBlackScreen || !newScreen.HasBlackScreen;
                            float openAnimationDuration;
                            if (delay > 0)
                                openAnimationDuration = delay;
                            else
                                openAnimationDuration = oldScreen.HasBlackScreen && !newScreen.HasBlackScreen ? 0 : oldScreen.OpenAnimationDuration;
                            oldScreen.OnHide(deactivate, openAnimationDuration);
                            _uiEventHandler.OnHideScreen(oldScreen);
                            break;
                        }
                    case EPreviosScreenAct.ForceClose:
                        {
                            _screenStack.RemoveAt(index);
                            oldScreen.ForceExit();
                            _uiEventHandler.OnScreenForceClose(oldScreen);
                            break;
                        }
                }
            }
        }

        private float CalculateFitterValue(float aspectRatio, float cameraAspect)
        {
            float tmp = (aspectRatio * cameraAspect) / BaseAspect;
            return tmp -= 0.015f;
        }

        private void CheckScreenInstatiateState(string screenName, RectTransform parent)
        {
            if (!_screenPool.ContainsKey(screenName))
            {
                _screenPool.Add(screenName, CreateScreen(screenName, parent));
            }
        }

        private UIScreenItem CreateScreen(string screenName, RectTransform parent)
        {
            int index = _screenItems.FindIndex(input => string.Compare(input.ScreenKey, screenName) == 0);
            if (index > -1 && index < _screenItems.Count)
            {
                UIScreenItem prefab = _screenItems[index].ScreenPrefab;
                UIScreenItem elementBuffer = GameObject.Instantiate(prefab, parent);
                elementBuffer.Initialization();
                elementBuffer.SetActivation(false);
                return elementBuffer;
            }

            Debug.LogError(screenName + " not found");
            return null;
        }

        private float CalculateScaleFactor()
        {
            float cameraAspect = (float)Screen.width / Screen.height;
            if (cameraAspect > 0.56)
                return 1;
            else
                return 0;
        }

        private void LoadCompelete(UIMainConfig uiMainConfig)
        {
            _screenItems = uiMainConfig.ScreenItems;
            _screenBackPanel = GameObject.Instantiate(uiMainConfig.ScreenBlackPanelPrefab);
            _lockPanel = GameObject.Instantiate(uiMainConfig.ScreenLockPanel);
            _mainCanvasTransform = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
            _screenCanvasTransform = GameObject.FindGameObjectWithTag("ScreenCanvas").GetComponent<RectTransform>();
            _screenCanvas = _screenCanvasTransform.GetComponent<Canvas>();
            _lockPanel.SetParent(_screenCanvasTransform);
            _lockPanel.gameObject.SetActive(false);
            int length = uiMainConfig.PopupPoolCount;
            if (!uiMainConfig.ToastPrefab)
                Debug.LogError("ToastPrefab has no assign object");
            _toastPool = new IToastItem[length];
            for (int i = 0; i < length; ++i)
            {
                _toastPool[i] = GameObject.Instantiate(uiMainConfig.ToastPrefab, _screenCanvasTransform).GetComponent<IToastItem>();
                _toastPool[i]?.Initialization();
            }

            _screenBackPanel.Initialization("ScreensBlackScreen", _screenCanvasTransform);

            CanvasScaler mainCanvasScaler = _mainCanvasTransform.GetComponent<CanvasScaler>();
            mainCanvasScaler.matchWidthOrHeight = CalculateScaleFactor();
            CanvasScaler screenCanvasScaler = _screenCanvasTransform.GetComponent<CanvasScaler>();
            screenCanvasScaler.matchWidthOrHeight = CalculateScaleFactor();
            _screenStack = new List<UIScreenItem>();
            _screenPool = new Dictionary<string, UIScreenItem>();
            Resources.UnloadAsset(uiMainConfig);
        }

        void IScreen.LockAllInputs(bool isLockBack)
        {
            if (isLockBack)
                _backLockState = true;
            _lockPanel.SetAsLastSibling();
            _lockPanel.gameObject.SetActive(true);
        }

        void IScreen.UnlockAllInputs(bool isUnlockBack)
        {
            if (isUnlockBack)
                _backLockState = false;
            _lockPanel.gameObject.SetActive(false);
        }
    }
}