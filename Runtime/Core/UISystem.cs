﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameWarriors.UIDomain.Data;
using GameWarriors.UIDomain.Abstraction;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameWarriors.UIDomain.Core
{
    /// <summary>
    /// This class provide all system feature like: set and config aspect ratio, handling toasts, handling screens stack and fire UI events.
    /// </summary>
    public class UISystem : IAspectRatio, IToast, IScreenStack, IUIOperation
    {
        private readonly IUIEventHandler _uiEventHandler;
        private readonly IDependencyInjector _dependencyInjector;
        private Dictionary<string, IScreenItem> _screenPool;
        private List<UIScreenItemData> _screenItems;
        private List<IScreenItem> _screenStack;
        private IToastItem[] _toastPool;
        private ImageBlackPanel _screenBackPanel;
        private Transform _lockPanel;
        private RectTransform _screenCanvasTransform;
        private RectTransform _mainCanvasTransform;
        private int _positionIndex;
        private bool _backLockState;

        //public float BaseAspect => 0.5629406f;
        public float BaseAspect => 0.56240487f;
        public float CurrentAspect => (float)Screen.width / (float)Screen.height;
        public float TopSideSafeAreaOffset => (Screen.height - Screen.safeArea.height) / 2;
        public IScreenItem LastScreen => _screenStack.Count > 0 ? _screenStack[_screenStack.Count - 1] as IScreenItem : null;
        public int OpenScreenCount => _screenStack.Count;


        [UnityEngine.Scripting.Preserve]
        public UISystem(IUIEventHandler eventHandler, IUIResources resources, IDependencyInjector dependencyInjector, IServiceProvider serviceProvider)
        {
            _uiEventHandler = eventHandler;
            if (resources == null)
                resources = new DefaultResourseLoader();

            if (dependencyInjector == null)
                dependencyInjector = new DefaultDependencyInjector(serviceProvider);

            _dependencyInjector = dependencyInjector;
            resources.LoadResourceAsync(UIMainConfig.RESOURCES_PATH, LoadComplete);
        }

        [UnityEngine.Scripting.Preserve]
        public async Task WaitForLoading()
        {
            while (_screenPool == null)
            {
                await Task.Delay(100);
            }
        }

        [UnityEngine.Scripting.Preserve]
        public IEnumerator WaitForLoadingCoroutine()
        {
            yield return new WaitUntil(() => _screenPool != null);
        }

        void IToast.ShowToast(string context, float lifeTime)
        {
            IToastItem toastItem = _toastPool[0];
            toastItem.SetData(context);
            _uiEventHandler.OnToastRises(lifeTime, toastItem);
        }

        void IToast.ShowToast(string context, EToastPlace typeMessage, float lifeTime)
        {
            IToastItem toastItem = _toastPool[_positionIndex];
            toastItem.SetData(context);
            if (typeMessage == EToastPlace.Up)
                toastItem.Position = new Vector2(0, 550 + (_positionIndex * 70));
            else
                if (typeMessage == EToastPlace.Mid)
                toastItem.Position = new Vector2(0, (_positionIndex * 70));
            else
                toastItem.Position = new Vector2(0, -550 + (_positionIndex * 70));
            ++_positionIndex;
            _positionIndex %= _toastPool.Length;
            _uiEventHandler.OnToastRises(lifeTime, toastItem);
        }

        void IScreenStack.SetBackLockState(bool state)
        {
            _backLockState = state;
        }

        void IScreenStack.ShowScreen(string screenName, ECanvasType canvasType, EPreviousScreenAct previousScreenAct, Action onClose)
        {
            ShowScreen<IScreenItem>(screenName, canvasType, previousScreenAct, onClose);
        }

        public T ShowScreen<T>(string screenName, ECanvasType canvasType, EPreviousScreenAct previousScreenAct, Action onClose = default) where T : IScreenItem
        {
            if (string.IsNullOrEmpty(screenName))
                return default;
            RectTransform parent = canvasType == ECanvasType.MainCanvas ? _mainCanvasTransform : _screenCanvasTransform;
            CheckScreenInsatiateState(screenName, parent);

            IScreenItem element = _screenPool[screenName];
            if (element.HasBlackScreen)
                _screenBackPanel.ShowIn();
            else
                _screenBackPanel.Activation = false;

            bool isOpen = IsScreenInList(screenName);
            if (isOpen)
            {
                element = CreateScreen(screenName, parent);
                element.IsDestroy = true;
            }

            SetPreviousScreenState(previousScreenAct, element);

            _screenStack.Add(element);
            element.OnShow(onClose);
            _uiEventHandler.OnOpenScreen(element);
            return (T)element;
        }

        public void CloseScreen(string screenName, bool showOpenAnimation = true)
        {
            if (string.IsNullOrEmpty(screenName))
                return;

            for (int i = _screenStack.Count - 1; i > -1; --i)
            {
                if (string.Compare(_screenStack[i].ScreenName, screenName) == 0)
                {
                    IScreenItem closeElement = _screenStack[i];
                    _screenStack.RemoveAt(i);

                    IScreenItem lastOpenScreen = null;
                    if (_screenStack.Count > 0)
                        lastOpenScreen = _screenStack[_screenStack.Count - 1];

                    closeElement.OnClose();
                    _uiEventHandler.OnCloseScreen(closeElement);

                    IScreenItem openScreen = null;
                    if (_screenStack.Count > 0)
                        openScreen = _screenStack[_screenStack.Count - 1];
                    else if (closeElement.HasBlackScreen)
                        _screenBackPanel.DisableScreen();

                    if (openScreen != null && lastOpenScreen == openScreen)
                    {
                        if (!openScreen.HasBlackScreen)
                        {
                            if (_screenBackPanel.Activation)
                            {
                                float length = closeElement.CloseAnimationDuration;
                                if (length > 0)
                                    _screenBackPanel.FadeOut(null);
                                else
                                    _screenBackPanel.DisableScreen();
                            }
                        }
                        else if (!_screenBackPanel.Activation)
                            _screenBackPanel.ShowIn();

                        showOpenAnimation = showOpenAnimation && !closeElement.HasBlackScreen;
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
            IScreenItem mainMenuScreen = _screenStack[0];
            int startIndex = isCloseLastScreen ? 0 : 1;
            for (int i = startIndex; i < length; ++i)
            {
                _screenStack[i].OnClose(-1);
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
            for (int i = 0; i < length; ++i)
            {
                if (_screenStack[i].ScreenName == targetName)
                    return true;
            }
            return false;
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

        void IUIOperation.LockAllInputs(bool isLockBack)
        {
            if (isLockBack)
                _backLockState = true;
            _lockPanel.SetAsLastSibling();
            _lockPanel.gameObject.SetActive(true);
        }

        void IUIOperation.UnlockAllInputs(bool isUnlockBack)
        {
            if (isUnlockBack)
                _backLockState = false;
            _lockPanel.gameObject.SetActive(false);
        }

        void IUIOperation.ChangeCanvasCamera(Camera newCamera)
        {
            _mainCanvasTransform.GetComponent<Canvas>().worldCamera = newCamera;
            _screenCanvasTransform.GetComponent<Canvas>().worldCamera = newCamera;
            _uiEventHandler.OnCanvasCameraChange(newCamera);
        }

        void IUIOperation.SystemUpdate()
        {

        }

        public Transform ShowBlackScreen()
        {
            _screenBackPanel.ShowIn();
            return _screenBackPanel.TransformRef;
        }

        public void HideBlackScreen(Action hideBlackPanelDone)
        {
            _screenBackPanel.FadeOut(hideBlackPanelDone);
        }

        public T FindScreenInStack<T>(string screenName) where T : IScreenItem
        {
            int count = _screenStack?.Count ?? 0;
            for (int i = 0; i < count; ++i)
            {
                if (_screenStack[i].ScreenName == screenName)
                {
                    return (T)_screenStack[i];
                }
            }
            return default;
        }

        void IUIOperation.ApplyBackButton()
        {
            if (!_backLockState)
            {
                int lastIndex = _screenStack.Count - 1;
                if (lastIndex > 0)
                {
                    IScreenItem lastScreen = _screenStack[lastIndex];
                    lastScreen.OnRequestCloseScreen(false, true);
                }
                else if (lastIndex == 0)
                {
                    IScreenItem lastScreen = _screenStack[lastIndex];
                    lastScreen.OnRequestCloseScreen(true, false);
                    _uiEventHandler.OnCloseLastScreen();
                }
            }
        }

        private void SetPreviousScreenState(EPreviousScreenAct previosScreenAct, IScreenItem newScreen, float delay = 0)
        {
            if (_screenStack.Count > 0)
            {
                int index = _screenStack.Count - 1;
                IScreenItem oldScreen = _screenStack[index];
                switch (previosScreenAct)
                {
                    case EPreviousScreenAct.Close:
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
                    case EPreviousScreenAct.Queue:
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
                    case EPreviousScreenAct.ForceClose:
                        {
                            _screenStack.RemoveAt(index);
                            oldScreen.OnClose(-1);
                            _uiEventHandler.OnScreenForceClose(oldScreen);
                            break;
                        }
                        case EPreviousScreenAct.None:
                        {
                            oldScreen.OnHide(false, 0);
                            _uiEventHandler.OnHideScreen(oldScreen);
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

        private void CheckScreenInsatiateState(string screenName, RectTransform parent)
        {
            if (!_screenPool.ContainsKey(screenName))
            {
                _screenPool.Add(screenName, CreateScreen(screenName, parent));
            }
        }

        private IScreenItem CreateScreen(string screenName, RectTransform parent)
        {
            int index = _screenItems.FindIndex(input => string.Compare(input.ScreenKey, screenName) == 0);
            if (index > -1 && index < _screenItems.Count)
            {
                MonoBehaviour prefab = _screenItems[index].ScreenPrefab;
                IScreenItem elementBuffer = (IScreenItem)UnityEngine.Object.Instantiate(prefab, parent);
                _dependencyInjector.Inject(elementBuffer);
                elementBuffer.OnInitialized();
                elementBuffer.SetActivation(false);
                return elementBuffer;
            }
            throw new KeyNotFoundException(screenName + " not found");
        }

        private float CalculateScaleFactor()
        {
            float cameraAspect = (float)Screen.width / Screen.height;
            if (cameraAspect > 0.56)
                return 1;
            else
                return 0;
        }

        private void LoadComplete(UIMainConfig uiMainConfig)
        {
            _screenItems = uiMainConfig.ScreenItems;
            _screenBackPanel = GameObject.Instantiate(uiMainConfig.ScreenBlackPanelPrefab);
            _lockPanel = GameObject.Instantiate(uiMainConfig.ScreenLockPanel);
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            if (mainCanvas == null && uiMainConfig.MainCanvasPrefab != null)
                _mainCanvasTransform = GameObject.Instantiate(uiMainConfig.MainCanvasPrefab).GetComponent<RectTransform>();
            else if (mainCanvas != null)
                _mainCanvasTransform = mainCanvas.GetComponent<RectTransform>();
            else
                throw new KeyNotFoundException("The main canvas has not found");

            GameObject screenCanvas = GameObject.FindGameObjectWithTag("ScreenCanvas");
            if (screenCanvas == null && uiMainConfig.ScreenCanvasPrefab != null)
                _screenCanvasTransform = GameObject.Instantiate(uiMainConfig.ScreenCanvasPrefab).GetComponent<RectTransform>();
            else if (screenCanvas != null)
                _screenCanvasTransform = screenCanvas.GetComponent<RectTransform>();
            else
                throw new KeyNotFoundException("The screen canvas has not found");

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
            _screenStack = new List<IScreenItem>();
            _screenPool = new Dictionary<string, IScreenItem>();
        }
    }
}