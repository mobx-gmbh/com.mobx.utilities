using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public class SerializedPropertyInspectorMember : InspectorMember
    {
        private readonly UnityEditor.SerializedObject _serializedObject;
        private readonly UnityEditor.SerializedProperty _serializedProperty;
        private readonly bool _hideLabel;
        private readonly bool _runtimeReadonly;
        private readonly bool _readonly;
        private readonly ReorderableList _reorderableList;

        public SerializedPropertyInspectorMember(UnityEditor.SerializedProperty serializedProperty,
            MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _serializedProperty = serializedProperty ?? throw new ArgumentNullException(nameof(serializedProperty));
            _serializedObject = serializedProperty.serializedObject;
            _hideLabel = memberInfo.HasAttribute<HideLabelAttribute>();
            _runtimeReadonly = memberInfo.HasAttribute<RuntimeReadonlyAttribute>();
            _readonly = memberInfo.HasAttribute<ReadonlyInspectorAttribute>();

            var label = memberInfo.TryGetCustomAttribute(out LabelAttribute labelAttribute)
                ? labelAttribute.Label
                : serializedProperty.name.Humanize(Prefixes);

            Label = new GUIContent(label, serializedProperty.tooltip);
        }

        protected override void DrawGUI()
        {
            if (EngineCallbacks.EngineState == 3)
            {
                return;
            }

            _serializedObject.Update();
            var enabled = GUI.enabled;
            if (_readonly || _runtimeReadonly && Application.isPlaying)
            {
                GUI.enabled = false;
            }

            if (_hideLabel)
            {
                UnityEditor.EditorGUILayout.PropertyField(_serializedProperty, GUIContent.none);
                GUI.enabled = enabled;
                _serializedObject.ApplyModifiedProperties();
                return;
            }

            UnityEditor.EditorGUILayout.PropertyField(_serializedProperty, Label);
            _serializedObject.ApplyModifiedProperties();

            GUI.enabled = enabled;
        }
    }
}
