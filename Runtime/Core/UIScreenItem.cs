using System;
using GameWarriors.UIDomain.Abstraction;
using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace GameWarriors.UIDomain.Core
{
    public abstract class UIScreenItem : MonoBehaviour, IUIScreen
    {
        private static IScreen _screenHandler;
        private static IToast _toastNotification;
        private static IServiceProvider _serviceProvider;

        private Action _onClose;

        public IScreen ScreenHandler => _screenHandler;
        public IToast ToastNotification => _toastNotification;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public virtual float OpenAnimationDuration
        {
            get
            {
                if (Animation != null)
                {
                    AnimationClip clip = Animation.GetClip(OpenScreenAnimationName);
                    return clip != null ? clip.length : 0;
                }
                return 0;
            }
        }

        public virtual float CloseAnimationDuration
        {
            get
            {
                if (Animation != null)
                {
                    AnimationClip clip = Animation.GetClip(CloseScreenAnimationName);
                    return clip != null ? clip.length : 0;
                }
                return 0;
            }
        }

        public bool IsDestroy { get; set; }
        public abstract string ScreenName { get; }
        public virtual bool CanCloseByBack => true;
        public virtual bool HasBlackScreen => true;
        public virtual bool IsActive => gameObject.activeSelf;
        public virtual bool HasLogEvent => true;

        protected Animation Animation { get; private set; }
        protected virtual string OpenScreenAnimationName => "PanelAnimationOpen";
        protected virtual string CloseScreenAnimationName => "PanelAnimationClose";


        public static void Initialization(IScreen screenHandler, IServiceProvider serviceProvider, IToast ToastNotificationHandler)
        {
            _screenHandler = screenHandler;
            _serviceProvider = serviceProvider;
            _toastNotification = ToastNotificationHandler;
        }

        /// <summary>
        /// The method triggers just in the screen first time call and after instantiation of screen prefab
        /// </summary>
        public virtual void OnInitialized()
        {
            Animation = GetComponent<Animation>();
            transform.localScale = Vector3.one;
            //if (_animation)
            //    _animation.playAutomatically = false;
        }

        /// <summary>
        /// The method triggers each time screen enabled and shows in UI canvas and be a last screen in screen stack
        /// </summary>
        /// <param name="onClose">assigning the action method call to triggers after screen closed</param>
        /// <param name="showAnimation">to open the screen by playing open screen animation</param>
        public virtual void OnShow(Action onClose = null, bool showAnimation = true)
        {
            if (showAnimation)
            {
                PlayAnimation(OpenScreenAnimationName);
            }
            if (onClose != null)
                _onClose = onClose;
            transform.SetAsLastSibling();
            StopAllCoroutines();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// The method triggers when the new screen shows up and the screen goes behind the new screen.
        /// </summary>
        /// <param name="isDeactivate">true = the game object disappear by visibility, false = just hide logically</param>
        /// <param name="delay">if the isDeactivate be true, apply delay to disabling game object. default value is close animation duration</param>
        public virtual void OnHide(bool isDeactivate, float delay = 0)
        {
            if (isDeactivate)
            {
                if (delay <= 0)
                    delay = CloseAnimationDuration;

                if (delay > 0)
                {
                    StartCoroutine(WaitAndAction(delay, Deactivate));
                    PlayAnimation(CloseScreenAnimationName);
                }
                else
                    gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// The method triggers when screen close and remove from screen stack.
        /// </summary>
        /// <param name="delay">apply delay to disabling game object. default value is close animation duration</param>
        public virtual void OnClose(float delay = 0)
        {
            _onClose?.Invoke();
            _onClose = null;
            if (delay == 0)
                delay = CloseAnimationDuration;
            if (delay > 0)
            {
                PlayAnimation(CloseScreenAnimationName);
                StartCoroutine(WaitAndAction(delay, Deactivate));
            }
            else
                ForceExit();
        }

        /// <summary>
        /// The method triggers when the screen request for closing by device back button
        /// </summary>
        /// <param name="isLastScreen">is the current screen is the last screen in stack</param>
        /// <param name="showOpenAnimation">to open the screen by playing open screen animation</param>
        public virtual void OnRequestCloseScreen(bool isLastScreen, bool showOpenAnimation)
        {
            if (CanCloseByBack && !isLastScreen)
                ScreenHandler.CloseScreen(ScreenName, showOpenAnimation);
        }

        public void SetActivation(bool state)
        {
            gameObject.SetActive(state);
            if (state)
                transform.SetAsLastSibling();
            else if (!state && IsDestroy)
            {
                Destroy(this.gameObject);
            }
        }

        protected void ForceExit()
        {
            if (!IsDestroy)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

        protected IEnumerator WaitAndAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void PlayAnimation(string animationName)
        {
            if (Animation)
            {
                Animation.Stop();
                Animation.Play(animationName);
            }
        }
    }
}