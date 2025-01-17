﻿using System.Collections.Generic;
using GameWarriors.UIDomain.Data;
using GameWarriors.UIDomain.Core;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using GameWarriors.UIDomain.Abstraction;

namespace GameWarriors.UIDomain.MainUIEditor
{
    public class UIMainConfigEditorWindow : EditorWindow
    {
        private const int MAX_SCREEN_NAME_LENGTH = 30;
        private List<UIScreenItemData> _itemPrefabList;
        private List<int> _searchIndex;
        private Canvas _mainCanvas;
        private Canvas _screenCanvas;
        private ImageBlackPanel _screenBlackPanel;
        private Transform _screenLockPanel;
        private RectTransform _toastPrefab;
        private int _toastPoolCount;

        private Vector2 _scrollViewRect;
        private string _searchPattern;
        private string _assetPath;

        public bool IsInSearch => !string.IsNullOrEmpty(_searchPattern);

        [MenuItem("Tools/UI Main Config")]
        private static void OpenUIMainConfigEditorWindow()
        {
            if (!Directory.Exists("Assets/AssetData/Resources/"))
                Directory.CreateDirectory("Assets/AssetData/Resources/");

            UIMainConfigEditorWindow window = (UIMainConfigEditorWindow)EditorWindow.GetWindow(typeof(UIMainConfigEditorWindow));
            window.Initialization(UIMainConfig.ASSET_PATH);
            window.Show();
        }

        public void Initialization(string assetPath)
        {
            _assetPath = assetPath;
            UIMainConfig asset = AssetDatabase.LoadAssetAtPath<UIMainConfig>(assetPath);
            if (asset == null)
            {
                asset = CreateInstance<UIMainConfig>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            if (asset.ScreenItems != null)
                _itemPrefabList = new List<UIScreenItemData>(asset.ScreenItems);
            else
                _itemPrefabList = new List<UIScreenItemData>();

            _toastPrefab = asset.ToastPrefab;
            _toastPoolCount = asset.PopupPoolCount;
            _screenBlackPanel = asset.ScreenBlackPanelPrefab;
            _screenLockPanel = asset.ScreenLockPanel;
            _screenCanvas = asset.ScreenCanvasPrefab;
            _mainCanvas = asset.MainCanvasPrefab;
            _searchPattern = string.Empty;
        }

        void OnGUI()
        {
            GUILayout.Label("UI Main Configuration", EditorStyles.boldLabel);
            _mainCanvas = EditorGUILayout.ObjectField("Main Canvas", _mainCanvas, typeof(Canvas), true) as Canvas;
            _screenCanvas = EditorGUILayout.ObjectField("Screen Canvas", _screenCanvas, typeof(Canvas), true) as Canvas;

            _screenBlackPanel = EditorGUILayout.ObjectField("Screen Black Panel", _screenBlackPanel, typeof(ImageBlackPanel), true) as ImageBlackPanel;
            _screenLockPanel = EditorGUILayout.ObjectField("Screen Lock Panel", _screenLockPanel, typeof(Transform), true) as Transform;
            DrawPopupNofiticationConfig();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            _scrollViewRect = EditorGUILayout.BeginScrollView(_scrollViewRect, GUILayout.Height(position.height - 150), GUILayout.Width(position.width));
            DrawScreenPrefabList();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
        }

        private void DrawPopupNofiticationConfig()
        {
            _toastPrefab = EditorGUILayout.ObjectField("Toast Prefab", _toastPrefab, typeof(RectTransform), true) as RectTransform;
            _toastPoolCount = EditorGUILayout.IntField("Toast Pool Count", _toastPoolCount);
        }

        private void DrawScreenPrefabList()
        {
            DrawSearchField();
            if (_itemPrefabList == null)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width * 0.125f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(position.width * 0.75f));
            EditorGUILayout.Space();
            bool isInSearch = IsInSearch;
            int count = IsInSearch ? _searchIndex.Count : _itemPrefabList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    GUILayout.BeginVertical(GUI.skin.button);
                    int index = isInSearch ? _searchIndex[i] : i;
                    UIScreenItemData itemData = _itemPrefabList[index];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Screen Name:", GUILayout.Width(148));
                    itemData.ScreenKey = GUILayout.TextField(itemData.ScreenKey, maxLength: MAX_SCREEN_NAME_LENGTH);
                    GUILayout.EndHorizontal();
                    MonoBehaviour screenItem = EditorGUILayout.ObjectField("Screen Prefab:", itemData.ScreenPrefab, typeof(MonoBehaviour), true) as MonoBehaviour;
                    if (screenItem is IScreenItem)
                    {
                        itemData.ScreenPrefab = screenItem;
                        _itemPrefabList[index] = itemData;
                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Remove Screen Item"))
                    {
                        _itemPrefabList.RemoveAt(index);
                        ClearSearchPatten();
                        GUILayout.EndVertical();
                        break;
                    }
                    GUILayout.EndVertical();
                }
                DrawAddItemSection();
            }
            else if (IsInSearch)
            {
                DrawCenterLable("Item Not Found");
            }
            else
            {
                DrawCenterLable("No Item Exist");
                DrawAddItemSection();
            }


            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Configuration Asset"))
            {
                UIMainConfig asset = AssetDatabase.LoadAssetAtPath<UIMainConfig>(_assetPath);
                asset.SetScreensData(_itemPrefabList);
                asset.SetToastNotificationData(_toastPrefab, _toastPoolCount);
                asset.SetPanelData(_screenBlackPanel, _screenLockPanel);
                asset.SetCanvasPrefabs(_mainCanvas, _screenCanvas);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }

        }

        private void DrawCenterLable(string context)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(context);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawAddItemSection()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Screen Item"))
            {
                _itemPrefabList.Add(new UIScreenItemData(string.Empty));
            }
        }

        private void DrawSearchField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search Name : ", GUILayout.Width(90));
            string newPattern = GUILayout.TextField(_searchPattern, maxLength: MAX_SCREEN_NAME_LENGTH);
            if (newPattern != _searchPattern)
            {
                ApplySearchPatten(newPattern);
            }
            if (!string.IsNullOrEmpty(_searchPattern))
            {
                int size = 20;
                if (GUILayout.Button("X", GUILayout.Width(size), GUILayout.Height(size)))
                {
                    ClearSearchPatten();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ClearSearchPatten()
        {
            _searchIndex?.Clear();
            _searchPattern = string.Empty;
        }

        private void ApplySearchPatten(string newPattern)
        {
            if (_itemPrefabList == null)
                return;

            _searchPattern = newPattern;
            int count = Mathf.Max(_itemPrefabList.Count, 5);
            _searchIndex ??= new List<int>(count);
            _searchIndex.Clear();
            int length = _itemPrefabList.Count;
            for (int i = 0; i < length; ++i)
            {
#if UNITY_2021_1_OR_NEWER
                if (_itemPrefabList[i].ScreenKey.Contains(newPattern, StringComparison.OrdinalIgnoreCase))
#else
                if (_itemPrefabList[i].ScreenKey.Contains(newPattern))
#endif
                    _searchIndex.Add(i);
            }
        }
    }
}