using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    [UnityEditor.CustomPropertyDrawer(typeof(InlineInspectorAttribute))]
    internal class InlineInspectorDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.Editor _inspector;
        private InlineInspectorAttribute _inspectorAttribute;
        private string _key;
        private bool _required;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (_required && property.objectReferenceValue == null)
            {
                var message = $"{property.displayName} is Required!";
                UnityEditor.EditorGUILayout.HelpBox(message, UnityEditor.MessageType.Error);
            }

            UnityEditor.EditorGUILayout.PropertyField(property, label);

            if (property.propertyType != UnityEditor.SerializedPropertyType.ObjectReference)
            {
                return;
            }

            if (attribute is InlineInspectorAttribute inlineInspectorAttribute)
            {
                _required = inlineInspectorAttribute.Required;
            }

            if (property.objectReferenceValue == null)
            {
                _inspector = null;
                return;
            }

            if (ReferenceEquals(property.objectReferenceValue, property.serializedObject.targetObject))
            {
                return;
            }

            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;
            GetOrCreateInspector(property).OnInspectorGUI();
        }

        private UnityEditor.Editor GetOrCreateInspector(UnityEditor.SerializedProperty property)
        {
            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;

            if (_inspector && _inspector.target != property.objectReferenceValue)
            {
                _inspector = null;
            }
            _inspector ??= UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

            return _inspector;
        }
    }
}