using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(RuntimeGUID))]
    public class RuntimeGUIDPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.SerializedProperty _stringProperty;
        private bool _isGuidSet;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            _stringProperty ??= property.FindPropertyRelative("value");
            GUIHelper.BeginEnabledOverride(false);
            label.text = "GUID";
            UnityEditor.EditorGUI.TextField(position, label, _stringProperty.stringValue);
            GUIHelper.EndEnabledOverride();

            if (_isGuidSet)
            {
                return;
            }
            UpdateGuid(property);
        }

        private void UpdateGuid(UnityEditor.SerializedProperty property)
        {
            if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(property.serializedObject.targetObject))
            {
                var prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(property.serializedObject.targetObject);
                var prefabGuid = UnityEditor.AssetDatabase.AssetPathToGUID(prefabPath);
                _stringProperty.serializedObject.Update();
                _stringProperty.stringValue = prefabGuid;
                _stringProperty.serializedObject.ApplyModifiedProperties();
                _isGuidSet = true;
                return;
            }
            var path = UnityEditor.AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            _stringProperty.stringValue = guid;
            _isGuidSet = true;
        }
    }
}
