using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public enum EToastPlace
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
        void ShowToast(string context);
        void ShowToast(string context, EToastPlace typeMessage);
    }
}