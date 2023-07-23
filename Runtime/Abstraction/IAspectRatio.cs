using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    /// <summary>
    /// The base abstraction which presents some useful values for fitting and adjusting UI items.
    /// </summary>
    public interface IAspectRatio
    {
        float BaseAspect { get; }
        float TopSideSafeAreaOffset { get; }
        float CurrentAspect { get; }
        void AdjustAspect(GameObject gameObject);
    }
}