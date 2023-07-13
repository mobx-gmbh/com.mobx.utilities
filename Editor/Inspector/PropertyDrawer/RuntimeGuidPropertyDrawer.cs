using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(RuntimeGuid))]
    public class RuntimeGuidPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.SerializedProperty _stringProperty;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            _stringProperty ??= property.FindPropertyRelative("value");
            GUIHelper.BeginEnabledOverride(false);
            UnityEditor.EditorGUI.TextField(position, label, _stringProperty.stringValue);
            GUIHelper.EndEnabledOverride();

            // TODO: Add button to update GUID manually

            if (_stringProperty.stringValue.IsNullOrWhitespace())
            {
                UpdateGuid(property);
            }
        }

        private void UpdateGuid(UnityEditor.SerializedProperty property)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            _stringProperty.stringValue = guid;
        }
    }
}
