using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUIOperation
    {
        void LockAllInputs(bool isLockBack = true);
        void UnlockAllInputs(bool isUnlockBack = true);
        void ChangeCanvasCamera(Camera newCamera);
        void SystemUpdate();
    }
}
