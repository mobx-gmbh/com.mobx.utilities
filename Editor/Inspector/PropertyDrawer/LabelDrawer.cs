using MobX.Utilities.Inspector;
using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public class LabelDrawer : UnityEditor.PropertyDrawer
    {
        private string _label;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _label ??= ((LabelAttribute) attribute).Label;
            label.text = _label;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}