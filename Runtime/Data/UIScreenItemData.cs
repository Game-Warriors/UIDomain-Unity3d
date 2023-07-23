using System;
using UnityEngine;

namespace GameWarriors.UIDomain.Data
{
    [Serializable]
    public struct UIScreenItemData
    {
        [SerializeField]
        private string _screenKey;
        [SerializeField]
        private MonoBehaviour _screenPrefab;

        public string ScreenKey { get => _screenKey; set => _screenKey = value; }
        public MonoBehaviour ScreenPrefab { get => _screenPrefab; set => _screenPrefab = value; }

        public UIScreenItemData(string key)
        {
            _screenKey = key;
            _screenPrefab = null;
        }
    }
}
