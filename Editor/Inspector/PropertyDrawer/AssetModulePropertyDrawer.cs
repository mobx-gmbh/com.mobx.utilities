using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(SubassetList<>))]
    public class AssetModulePropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var assets = property.FindPropertyRelative("assets");

            if (property.serializedObject.targetObject is not ScriptableObject)
            {
                GUIHelper.BoldLabel("Asset Modules is only available for ScriptableObjects!");
                return;
            }

            GUIHelper.BoldLabel(fieldInfo.FieldType.GetNameWithoutGenericArity());
        }
    }
}