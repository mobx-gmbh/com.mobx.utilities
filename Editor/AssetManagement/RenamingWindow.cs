﻿using MobX.Utilities.Editor.Helper;
using UnityEngine;

namespace MobX.Utilities.Editor.AssetManagement
{
    public class RenamingWindow : UnityEditor.EditorWindow
    {
        private UnityEditor.Editor _editor;
        private Vector2 _scrollPosition;

        [UnityEditor.MenuItem("Tools/Asset Renaming")]
        public static void ShowWindow()
        {
            GetWindow<RenamingWindow>("Asset and Object Renaming");
        }

        private void OnEnable()
        {
            _editor = UnityEditor.Editor.CreateEditor(AssetRenamingTool.Local);
            UnityEditor.Selection.selectionChanged += Repaint;
        }

        private void OnDisable()
        {
            UnityEditor.Selection.selectionChanged -= Repaint;
        }

        protected void OnGUI()
        {
            _scrollPosition = UnityEditor.EditorGUILayout.BeginScrollView(_scrollPosition);
            GUIHelper.BeginLabelWidthOverride(250);
            if (_editor != null)
            {
                _editor.OnInspectorGUI();
            }
            GUIHelper.EndLabelWidthOverride();
            UnityEditor.EditorGUILayout.EndScrollView();
        }
    }
}
