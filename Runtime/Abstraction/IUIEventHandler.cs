using System;

using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IUIEventHandler
    {
        void SetUIUpdate(Action uiUpdate);
        void OnToastRises(float showTimeLength, IToastItem toast);
        void OnCloseLastScreen();
        void OnShowScreen(IUIScreen screen);
        void OnCloseScreen(IUIScreen screen);
        void OnOpenScreen(IUIScreen screen);
        void OnHideScreen(IUIScreen screen);
        void OnScreenForceClose(IUIScreen screen);
    }
}