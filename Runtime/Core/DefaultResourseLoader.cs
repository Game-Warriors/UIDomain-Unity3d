using System;
using GameWarriors.UIDomain.Data;
using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    /// <summary>
    /// This class provide resource loading for UI system
    /// </summary>
    public class DefaultResourseLoader : IUIResources
    {
        public void LoadResourceAsync(string assetName, Action<UIMainConfig> onLoadDone)
        {
            ResourceRequest operation = Resources.LoadAsync<UIMainConfig>(assetName);
            operation.completed += (asyncOperation) =>
            {
                UIMainConfig asset = (UIMainConfig)(asyncOperation as ResourceRequest).asset;
                onLoadDone(asset);
                Resources.UnloadAsset(asset);
            };
        }
    }
}