﻿using System;
using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public enum ECanvasType { MainCanvas, ScreenCanvas }
    public enum EPreviousScreenAct { None, Close, Queue, ForceClose }

    /// <summary>
    /// The base abstraction which presents screen stack features like, show, close, working with black screen and control screen remove by system back.
    /// </summary>
    public interface IScreenStack
    {
        /// <summary>
        /// return last open screen in screen stack
        /// </summary>
        IScreenItem LastScreen { get; }

        /// <summary>
        /// return count of open screen in stack
        /// </summary>
        int OpenScreenCount { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenName">the screen name which is booked in UI editor tools for each screen prefab</param>
        /// <param name="canvasType">the target canvas which desire the screen prefab attached to. it can be Main or Screen</param>
        /// <param name="previousScreenAct">indicate what happen to previous screen after showing desire screen</param>
        /// <param name="onClose">assigning the action method call to triggers after screen closed</param>
        void ShowScreen(string screenName, ECanvasType canvasType, EPreviousScreenAct previousScreenAct, Action onClose = default);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="screenName">the screen name which is booked in UI editor tools for each screen prefab</param>
        /// <param name="canvasType">the target canvas which desire the screen prefab attached to. it can be Main or Screen</param>
        /// <param name="previousScreenAct">indicate what happen to previous screen after showing desire screen</param>
        /// <param name="onClose">assigning the action method call to triggers after screen closed</param>
        /// <returns></returns>
        T ShowScreen<T>(string screenName, ECanvasType canvasType, EPreviousScreenAct previousScreenAct, Action onClose = default) where T : IScreenItem;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="showOpenAnimation"></param>
        void CloseScreen(string screenName, bool showOpenAnimation = true);
        Transform ShowBlackScreen();
        void HideBlackScreen(Action hideBlackPanelDone);
        /// <summary>
        /// Close all screens and clear screen stack.
        /// </summary>
        /// <param name="isCloseLastScreen"></param>
        void ClearScreens(bool isCloseLastScreen = true);
        bool IsScreenInList(string targetName);
        T FindScreenInStack<T>(string screenName) where T : IScreenItem;
        void SetBackLockState(bool state);
    }
}
