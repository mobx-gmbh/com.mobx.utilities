using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Editor.AssetIcons
{
    public class AssetIconSettingsProvider : UnityEditor.SettingsProvider
    {
        private UnityEditor.SerializedObject _mappingsObject;
        private UnityEditor.SerializedProperty _mappingsProperty;

        [UnityEditor.SettingsProviderAttribute]
        public static UnityEditor.SettingsProvider CreateSettingsProvider()
        {
            return new AssetIconSettingsProvider("Project/Asset Icons", UnityEditor.SettingsScope.Project);
        }

        private AssetIconSettingsProvider(string path, UnityEditor.SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            guiHandler = searchContext =>
            {
                UnityEditor.EditorGUILayout.LabelField("Initialization");
                AssetIconMappings mappings = AssetIconSettings.instance.Mappings;
                mappings = (AssetIconMappings) UnityEditor.EditorGUILayout.ObjectField("Mappings", mappings,
                    typeof(AssetIconMappings), false);
                AssetIconSettings.instance.Mappings = mappings;

                if (_mappingsProperty != null && mappings == null)
                {
                    _mappingsObject = null;
                    _mappingsProperty = null;
                }
                if (_mappingsProperty == null && mappings != null)
                {
                    _mappingsObject = new UnityEditor.SerializedObject(mappings);
                    _mappingsProperty = _mappingsObject.FindProperty("scriptIcons");
                }
                if (_mappingsProperty != null && _mappingsObject != null)
                {
                    _mappingsObject.Update();
                    UnityEditor.EditorGUILayout.PropertyField(_mappingsProperty);
                    _mappingsObject.ApplyModifiedProperties();
                }

                UnityEditor.EditorGUILayout.Space();
                if (GUILayout.Button("Update All Assets"))
                {
                    AssetIconSettings.instance.ValidateAllAssets();
                }

                if (GUI.changed)
                {
                    AssetIconSettings.instance.SaveSettings();
                }
            };

            keywords = new HashSet<string>(new[]
            {
                "Icons", "Script Icons"
            });
        }
    }
}
