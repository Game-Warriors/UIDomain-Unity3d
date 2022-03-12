using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IAspectRatio
    {
        float BaseAspect { get; }
        float TopSideSafeAreaOffset { get; }
        float CurrentAspect { get; }
        void AdjustAspect(GameObject gameObject);

    }
}