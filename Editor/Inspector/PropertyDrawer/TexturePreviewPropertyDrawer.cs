using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
    public class TexturePreviewPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var isObjectReference = property.propertyType == UnityEditor.SerializedPropertyType.ObjectReference;

            if (isObjectReference)
            {
                UnityEditor.EditorGUI.BeginProperty(position, label, property);
                property.objectReferenceValue =
                    GUIHelper.ObjectField(position, label, (Texture2D) property.objectReferenceValue, false);

                UnityEditor.EditorGUI.EndProperty();
            }
            else
            {
                UnityEditor.EditorGUI.LabelField(position, label.text, "Use PreviewTexture with Texture2D.");
            }
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            var previewAttribute = (TexturePreviewAttribute) attribute;
            return UnityEditor.EditorGUIUtility.singleLineHeight + previewAttribute.PreviewFieldHeight;
        }
    }
}