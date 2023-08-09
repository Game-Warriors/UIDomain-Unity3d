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
        /// <summary>
        /// Loading configuration data and call the action after operation done.
        /// </summary>
        /// <param name="assetName">Configuration data name</param>
        /// <param name="onLoadDone">Trigger action after data fully loaded</param>
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