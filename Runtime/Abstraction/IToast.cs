using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    /// <summary>
    /// present showing location of toast in central vertical line of screen
    /// </summary>
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

    /// <summary>
    /// The base abstraction which presents screen showing toast features which is specify by visibility time and screen point.
    /// </summary>
    public interface IToast
    {
        void ShowToast(string context, float lifeTime = 1.5f);
        void ShowToast(string context, EToastPlace typeMessage, float lifeTime = 1.5f);
    }
}