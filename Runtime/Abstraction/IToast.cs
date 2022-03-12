using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public enum MessagePopUpPlace
    {
        Up,
        Mid,
        Down
    }

    public interface IToastItem
    {
        bool Activation { get; set; }
        Vector2 Position { get; set; }

        void SetData(string context);
        void Initialization();
    }

    public interface IToast
    {
        void ShowStaticToast(string context);
        void ShowDynamicToast(string context, MessagePopUpPlace typeMessage);
    }
}