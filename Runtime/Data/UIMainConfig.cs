using System;
using System.Collections.Generic;
using GameWarriors.UIDomain.Core;
using UnityEngine;

namespace GameWarriors.UIDomain.Data
{
    [Serializable]
    public struct UIScreenItemData
    {
        [SerializeField]
        private string _screenKey;
        [SerializeField]
        private UIScreenItem _screenPrefab;

        public string ScreenKey { get => _screenKey; set => _screenKey = value; }
        public UIScreenItem ScreenPrefab { get => _screenPrefab; set => _screenPrefab = value; }

        public UIScreenItemData(string key)
        {
            _screenKey = key;
            _screenPrefab = null;
        }
    }

    public class UIMainConfig : ScriptableObject
    {
        public const string RESOURCES_PATH = "UIMainConfig";
        public const string ASSET_PATH = "Assets/AssetData/Resources/UIMainConfig.asset";
        [HideInInspector, SerializeField]
        private List<UIScreenItemData> _screenItems;
        [HideInInspector, SerializeField]
        public Canvas _mainCanvasPrefab;
        [HideInInspector, SerializeField]
        public Canvas _screenCanvasPrefab;
        [HideInInspector, SerializeField]
        private ImageBlackPanel _screenBlackPanelPrefab;
        [HideInInspector, SerializeField]
        private Transform _screenLockPanelPrefab;
        [HideInInspector, SerializeField]
        private RectTransform _popupPrefab;
        [HideInInspector, SerializeField]
        private int _popupPrefabCount;

        public List<UIScreenItemData> ScreenItems => _screenItems;
        public ImageBlackPanel ScreenBlackPanelPrefab => _screenBlackPanelPrefab;
        public int PopupPoolCount => _popupPrefabCount;
        public RectTransform ToastPrefab => _popupPrefab;
        public Transform ScreenLockPanel => _screenLockPanelPrefab;

        public Canvas MainCanvasPrefab => _mainCanvasPrefab;
        public Canvas ScreenCanvasPrefab => _screenCanvasPrefab;

        public void SetScreensData(List<UIScreenItemData> itemPrefabList)
        {
            if (_screenItems == null)
                _screenItems = new List<UIScreenItemData>(itemPrefabList.Count);
            else
                _screenItems.Clear();
            int length = itemPrefabList.Count;
            for (int i = 0; i < length; ++i)
            {
                if (!string.IsNullOrEmpty(itemPrefabList[i].ScreenKey) && itemPrefabList[i].ScreenPrefab != null)
                    _screenItems.Add(itemPrefabList[i]);
            }

        }

        public void SetToastNotificationData(RectTransform ToastNotificationPrefab, int ToastNotificationPoolCount)
        {
            _popupPrefab = ToastNotificationPrefab;
            _popupPrefabCount = ToastNotificationPoolCount;
        }

        public void SetPanelData(ImageBlackPanel screenBlackPanel, Transform lockPanelPrefab)
        {
            _screenBlackPanelPrefab = screenBlackPanel;
            _screenLockPanelPrefab = lockPanelPrefab;
        }

        public void SetCanvasPrefabs(Canvas mainCanvas, Canvas screenCanvas)
        {
            _mainCanvasPrefab = mainCanvas;
            _screenCanvasPrefab = screenCanvas;
        }
    }
}
