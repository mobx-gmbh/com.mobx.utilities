using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities
{
    public class AssetDatabaseUtilities : MonoBehaviour
    {
        [Pure]
        public static List<T> FindAssetsOfType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        [Pure]
        public static string[] GetAssetPathsOfType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            var paths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return paths;
        }

        /*
         * Non Generic
         */

        [Pure]
        public static List<ScriptableObject> FindAssetsOfType(Type objectType)
        {
            var assets = new List<ScriptableObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{objectType}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        [Pure]
        public static List<Component> FindPrefabsOfType(Type componentType)
        {
            var assets = new List<Component>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponents(componentType, out Component[] components))
                {
                    assets.AddRange(components);
                }
            }

            return assets;
        }

        public static string[] FindAllScenePaths()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");
            var scenePaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                scenePaths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return scenePaths;
        }
    }
}
