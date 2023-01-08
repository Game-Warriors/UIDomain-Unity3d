using System;
using GameWarriors.UIDomain.Core;
using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public enum ECanvasType { MainCanvas, ScreenCanvas }
    public enum EPreviosScreenAct { None, Close, Queue, ForceClose }

    public interface IScreen
    {
        IUIScreen LastScreen { get; }
        int OpenScreenCount { get; }

        void ShowScreen(string screenName, ECanvasType canvasType, EPreviosScreenAct previosScreenAct, Action onClose = default);
        T ShowScreen<T>(string screenName, ECanvasType canvasType, EPreviosScreenAct previosScreenAct, Action onClose = default) where T : UIScreenItem;
        void CloseScreen(string screenName, bool showOpenAnimation = true);
        Transform ShowBlackScreen(float length);
        void HideBlackScreen(float length, Action hideBlackPanelDone);
        void ClearScreens(bool isCloseLastScreen = true);
        void SetBackLockState(bool state);
        bool IsScreenInList(string targetName);
        T FindScreenInStack<T>(string screenName) where T : UIScreenItem;
        void LockAllInputs(bool isLockBack = true);
        void UnlockAllInputs(bool isUnlockBack = true);
        void ChangeCanvasCamera(Camera newCamera);
    }
}
