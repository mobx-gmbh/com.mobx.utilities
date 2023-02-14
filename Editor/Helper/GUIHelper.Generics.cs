﻿using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor
{
    public static partial class GUIHelper
    {
        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : Object
        {
            return (T) EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
        }
    }
}