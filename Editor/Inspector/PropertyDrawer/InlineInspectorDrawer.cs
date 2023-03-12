using MobX.Utilities.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    [UnityEditor.CustomPropertyDrawer(typeof(InlineInspectorAttribute))]
    class InlineInspectorDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.Editor _inspector;
        private InlineInspectorAttribute _inspectorAttribute;
        private string _key;

        private static readonly Dictionary<Object, FoldoutHandler> foldoutHandlers = new Dictionary<Object, FoldoutHandler>();

        private FoldoutHandler Foldout(Object target)
        {
            if (foldoutHandlers.TryGetValue(target, out FoldoutHandler handler))
            {
                return handler;
            }

            handler = new FoldoutHandler(target.name);
            foldoutHandlers.Add(target, handler);
            return handler;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, property, label);

            if (property.propertyType != UnityEditor.SerializedPropertyType.ObjectReference)
            {
                return;
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

            _inspectorAttribute ??= (InlineInspectorAttribute)attribute;
            GetOrCreateInspector(property).OnInspectorGUI();
            FoldoutHandler.EndStyleOverride();
        }

        private UnityEditor.Editor GetOrCreateInspector(UnityEditor.SerializedProperty property)
        {
            _inspectorAttribute ??= (InlineInspectorAttribute)attribute;

            if (_inspector && _inspector.target != property.objectReferenceValue)
            {
                _inspector = null;
            }
            _inspector ??= UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

            return _inspector;
        }
    }
}
