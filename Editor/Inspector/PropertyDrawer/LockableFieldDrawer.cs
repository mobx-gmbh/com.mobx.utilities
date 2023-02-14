using MobX.Utilities.Inspector;
using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(LockableFieldAttribute))]
    internal class LockableFieldDrawer : UnityEditor.PropertyDrawer
    {
        private bool _writable = false;
        private const int BUTTON_WIDTH = 50;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = GUI.enabled;
            GUI.enabled = _writable;
            var textPos = new Rect(position.x, position.y, position.width - BUTTON_WIDTH, position.height);
            EditorGUI.PropertyField(textPos, property, label);
            GUI.enabled = enabled;

            var buttonPos = new Rect(position.x + position.width - BUTTON_WIDTH, position.y, BUTTON_WIDTH, position.height);
            if (GUI.Button(buttonPos , _writable? "Lock" : "Edit"))
            {
                _writable = !_writable;
            }
        }
    }
}