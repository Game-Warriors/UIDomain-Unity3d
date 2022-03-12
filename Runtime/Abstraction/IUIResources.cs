using System;
using GameWarriors.UIDomain.Data;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IUIResources
    {
        void LoadResourceAsync(string assetName, Action<UIMainConfig> OnLoadDone);
    }
}