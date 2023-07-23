using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IUIEventHandler
    {
        void OnToastRises(float showTimeLength, IToastItem toast);
        void OnCloseLastScreen();
        void OnShowScreen(IScreenItem screen);
        void OnCloseScreen(IScreenItem screen);
        void OnOpenScreen(IScreenItem screen);
        void OnHideScreen(IScreenItem screen);
        void OnScreenForceClose(IScreenItem screen);
        void OnCanvasCameraChange(Camera newCamera);
    }
}