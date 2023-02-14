﻿using MobX.Utilities.Inspector;
using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute), true)]
    public class ReadonlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = enabled;
        }
    }
}