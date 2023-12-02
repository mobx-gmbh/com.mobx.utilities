using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(Seconds))]
    public class SecondsDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label);
        }
    }
}