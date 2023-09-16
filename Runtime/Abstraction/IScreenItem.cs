using System;

namespace GameWarriors.UIDomain.Abstraction
{
    public enum EScreenState { None, Open, Hide, Close }

    /// <summary>
    /// The base abstraction which presents screen stack features like, show, close, working with black screen and control screen remove by system back.
    /// </summary>
    public interface IScreenItem
    {
        bool HasBlackScreen { get; }
        string ScreenName { get; }
        bool HasLogEvent { get; }
        float CloseAnimationDuration { get; }
        float OpenAnimationDuration { get; }
        bool IsDestroy { get; set; }

        void SetActivation(bool state);
        /// <summary>
        /// The method triggers just in the screen first time call and after instantiation of screen prefab
        /// </summary>
        void OnInitialized();
        /// <summary>
        /// The method triggers each time screen enabled and shows in UI canvas and be a last screen in screen stack
        /// </summary>
        /// <param name="onClose">assigning the action method call to triggers after screen closed</param>
        /// <param name="showAnimation">to open the screen by playing open screen animation</param>
        void OnShow(Action onClose = null, bool showAnimation = true);
        /// <summary>
        /// The method triggers when ever screen disable and removed from screens stack.
        /// </summary>
        /// <param name="delay">if the isDeactivate be true, apply delay to disabling game object. default value is close animation duration, -1 value will immediately disable screen item</param>
        void OnClose(float delay = 0);
        /// <summary>
        /// The method triggers when the new screen shows up and the screen goes behind the new screen.
        /// </summary>
        /// <param name="isDeactivate">true = the game object disappear by visibility, false = just hide logically</param>
        /// <param name="delay">if the isDeactivate be true, apply delay to disabling game object. default value is close animation duration</param>
        void OnHide(bool isDeactivate, float delay = 0);
        /// <summary>
        /// The method triggers when the screen request for closing by device back button
        /// </summary>
        /// <param name="isLastScreen">is the current screen is the last screen in stack</param>
        /// <param name="showOpenAnimation">to open the screen by playing open screen animation</param>
        void OnRequestCloseScreen(bool isLastScreen, bool showOpenAnimation);
    }
}
