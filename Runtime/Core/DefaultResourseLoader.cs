using System;
using GameWarriors.UIDomain.Data;
using UnityEngine;

namespace GameWarriors.UIDomain.Abstraction
{
    public class DefaultResourseLoader : IUIResources
    {
        public void LoadResourceAsync(string assetName, Action<UIMainConfig> onLoadDone)
        {
            ResourceRequest operation = Resources.LoadAsync<UIMainConfig>(assetName);
            operation.completed += (asyncOperation) => onLoadDone((asyncOperation as ResourceRequest).asset as UIMainConfig);
        }
    }
}