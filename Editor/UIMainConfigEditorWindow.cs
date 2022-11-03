using System.Collections.Generic;
using GameWarriors.UIDomain.Data;
using GameWarriors.UIDomain.Core;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace GameWarriors.UIDomain.MainUIEditor
{
    public class UIMainConfigEditorWindow : EditorWindow
    {
        private List<UIScreenItemData> _itemPrefabList;
        private Canvas _mainCanvas;
        private Canvas _screenCanvas;
        private ImageBlackPanel _screenBlackPanel;
        private Transform _screenLockPanel;
        private RectTransform _popupNotificationPrefab;
        private int _popupNotificationPoolCount;

        private bool _isDataChange;
        private Vector2 _scrollViewRect;


        [MenuItem("Tools/UI Main Config")]
        private static void OpenUIMainConfigEditorWindow()
        {
            if (!Directory.Exists("Assets/AssetData/Resources/"))
                Directory.CreateDirectory("Assets/AssetData/Resources/");

            UIMainConfigEditorWindow window = (UIMainConfigEditorWindow)EditorWindow.GetWindow(typeof(UIMainConfigEditorWindow));
            window.Initialization();
            window.Show();
        }

        private void Initialization()
        {
            UIMainConfig asset = AssetDatabase.LoadAssetAtPath<UIMainConfig>(UIMainConfig.ASSET_PATH);
            if (asset == null)
            {
                asset = CreateInstance<UIMainConfig>();
                AssetDatabase.CreateAsset(asset, UIMainConfig.ASSET_PATH);
            }
            if (asset.ScreenItems != null)
                _itemPrefabList = new List<UIScreenItemData>(asset.ScreenItems);
            else
                _itemPrefabList = new List<UIScreenItemData>();
            _popupNotificationPrefab = asset.ToastPrefab;
            _popupNotificationPoolCount = asset.PopupPoolCount;
            _screenBlackPanel = asset.ScreenBlackPanelPrefab;
            _screenLockPanel = asset.ScreenLockPanel;
            _screenCanvas = asset.ScreenCanvasPrefab;
            _mainCanvas = asset.MainCanvasPrefab;
        }

        void OnGUI()
        {
            GUILayout.Label("UI Main Configuration", EditorStyles.boldLabel);
            _mainCanvas = EditorGUILayout.ObjectField("Main Canvas", _mainCanvas, typeof(Canvas), true) as Canvas;
            _screenCanvas = EditorGUILayout.ObjectField("Screen Canvas", _screenCanvas, typeof(Canvas), true) as Canvas;

            _screenBlackPanel = EditorGUILayout.ObjectField("Screen BlackPanel", _screenBlackPanel, typeof(ImageBlackPanel), true) as ImageBlackPanel;
            _screenLockPanel = EditorGUILayout.ObjectField("Screen LockPanel", _screenLockPanel, typeof(Transform), true) as Transform;
            DrawPopupNofiticationConfig();

            _scrollViewRect = EditorGUILayout.BeginScrollView(_scrollViewRect, GUILayout.Height(position.height - 150), GUILayout.Width(position.width));
            DrawScreenPrefabList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

        }

        private void DrawPopupNofiticationConfig()
        {
            _popupNotificationPrefab = EditorGUILayout.ObjectField("Popup Notification Prefab", _popupNotificationPrefab, typeof(RectTransform), true) as RectTransform;
            _popupNotificationPoolCount = EditorGUILayout.IntField("Popup Notification Pool Count", _popupNotificationPoolCount);
        }

        private void DrawScreenPrefabList()
        {
            if (_itemPrefabList == null)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.125f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(position.width * 0.75f));
            EditorGUILayout.Space();
            for (int i = 0; i < _itemPrefabList.Count; ++i)
            {
                GUILayout.BeginVertical(GUI.skin.button);
                UIScreenItemData itemData = _itemPrefabList[i];
                itemData.ScreenKey = GUILayout.TextField(itemData.ScreenKey);
                itemData.ScreenPrefab = EditorGUILayout.ObjectField("Screen Prefab", itemData.ScreenPrefab, typeof(UIScreenItem), true) as UIScreenItem;
                _itemPrefabList[i] = itemData;
                EditorGUILayout.Space();
                if (GUILayout.Button("Remove Screen Item"))
                    _itemPrefabList.RemoveAt(i);
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Screen Item"))
            {
                _itemPrefabList.Add(new UIScreenItemData());
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Configuration Asset"))
            {
                UIMainConfig asset = AssetDatabase.LoadAssetAtPath<UIMainConfig>(UIMainConfig.ASSET_PATH);
                asset.SetScreensData(_itemPrefabList);
                asset.SetToastNotificationData(_popupNotificationPrefab, _popupNotificationPoolCount);
                asset.SetPanelData(_screenBlackPanel, _screenLockPanel);
                asset.SetCanvasPrefabs(_mainCanvas, _screenCanvas);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }

        }
    }
}
