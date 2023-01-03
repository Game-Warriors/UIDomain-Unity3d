using System;
using GameWarriors.UIDomain.Abstraction;
using UnityEngine;
using System.Collections;

namespace GameWarriors.UIDomain.Core
{
    public abstract class UIScreenItem : MonoBehaviour, IUIScreen
    {
        private static IScreen _screenHandler;
        private static IToast _toastNotification;
        private static IServiceProvider _serviceProvider;

        private Action _onClose;
        protected Animation _animation;

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

        protected Animation Animation => _animation;
        protected virtual string OpenScreenAnimationName => "PanelAnimationOpen";
        protected virtual string CloseScreenAnimationName => "PanelAnimationClose";


        public static void Initialization(IScreen screenHandler, IServiceProvider serviceProvider, IToast ToastNotificationHandler)
        {
            _screenHandler = screenHandler;
            _serviceProvider = serviceProvider;
            _toastNotification = ToastNotificationHandler;
        }

        public virtual void Initialization()
        {
            _animation = GetComponent<Animation>();
            //if (_animation)
            //    _animation.playAutomatically = false;
        }

        public virtual void SetActivation(bool state)
        {
            gameObject.SetActive(state);
            if (state)
                transform.SetAsLastSibling();
            else
                if (!state && IsDestroy)
            {
                Destroy(this.gameObject);
            }
        }

        public virtual void OnShow(Action onClose = null, bool showAnimation = true)
        {
            if (showAnimation)
            {
                PlayAnimation(OpenScreenAnimationName);
            }

            _onClose = onClose;
            transform.SetAsLastSibling();
            StopAllCoroutines();
            gameObject.SetActive(true);
        }

        public virtual void OnHide(bool isDeactivate, float delay = 0)
        {
            if (isDeactivate)
            {
                if (delay <= 0)
                    delay = CloseAnimationDuration;

                if (delay > 0)
                {
                    StartCoroutine(WaitAndAction(delay, Deactive));
                    PlayAnimation(CloseScreenAnimationName);
                }
                else
                    gameObject.SetActive(false);
            }
        }

        public virtual void OnClose(float delay = 0)
        {
            _onClose?.Invoke();

            if (delay <= 0)
                delay = CloseAnimationDuration;
            if (delay > 0)
            {
                PlayAnimation(CloseScreenAnimationName);
                StartCoroutine(WaitAndAction(delay, Deactive));
            }
            else
                ForceExit();
        }

        public virtual void OnRequestCloseScreen(bool isLastScreen, bool showOpenAnimation)
        {
            if (CanCloseByBack && !isLastScreen)
                ScreenHandler.CloseScreen(ScreenName, showOpenAnimation);
        }

        public virtual void ForceExit()
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

        private void Deactive()
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