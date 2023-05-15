using MobX.Utilities.Editor.AssetManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.AssetIcons
{
    [UnityEditor.FilePathAttribute("ProjectSettings/AssetIconSettings.asset",
        UnityEditor.FilePathAttribute.Location.ProjectFolder)]
    public class AssetIconSettings : UnityEditor.ScriptableSingleton<AssetIconSettings>
    {
        [SerializeField] private AssetIconMappings mappings;

        public AssetIconMappings Mappings
        {
            get => mappings;
            set => mappings = value;
        }

        public void SaveSettings()
        {
            Save(true);
        }

        public void ValidateAllAssets()
        {
            if (mappings == null)
            {
                return;
            }

            HashSet<Object> dirtyAssetBuffer = HashSetPool<Object>.Get();
            foreach ((UnityEditor.MonoScript monoScript, Texture2D value) in mappings.ScriptIcons)
            {
                Type type = monoScript.GetClass();
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{type}");
                var paths = AssetDatabaseUtilities.GUIDsToPaths(guids);
                ValidateAssets(paths, type, value, ref dirtyAssetBuffer);
            }

            foreach (Object item in dirtyAssetBuffer)
            {
                UnityEditor.EditorUtility.SetDirty(item);
            }
            UnityEditor.AssetDatabase.SaveAssets();
            HashSetPool<Object>.Release(dirtyAssetBuffer);
        }

        public void ValidateAssetPaths(string[] assetPaths)
        {
            if (mappings == null)
            {
                return;
            }
            foreach (var path in assetPaths)
            {
                ValidateAssetAtPath(path);
            }
        }

        private void ValidateAssetAtPath(string path)
        {
            foreach ((UnityEditor.MonoScript monoScript, Texture2D icon) in mappings.ScriptIcons)
            {
                Type type = monoScript.GetClass();
                Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
                if (asset != null)
                {
                    UnityEditor.EditorGUIUtility.SetIconForObject(asset, icon);
                    UnityEditor.EditorUtility.SetDirty(asset);
                }
            }
        }

        public static void ValidateAssets(string[] importedAssets, Type type, Texture2D icon, ref HashSet<Object> dirtyAssetBuffer)
        {
            foreach (var path in importedAssets)
            {
                Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
                if (asset == null)
                {
                    continue;
                }

                UnityEditor.EditorGUIUtility.SetIconForObject(asset, icon);
                dirtyAssetBuffer.Add(asset);
                if (asset is ScriptableObject scriptableObject)
                {
                    var monoScript = UnityEditor.MonoScript.FromScriptableObject(scriptableObject);
                    UnityEditor.EditorGUIUtility.SetIconForObject(monoScript, icon);
                    dirtyAssetBuffer.Add(monoScript);
                }
            }
        }
    }
}
