using MobX.Utilities.Callbacks;
using MobX.Utilities.Registry;
using MobX.Utilities.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Unity
{
    public static class ScriptableObjectUtility
    {
        private static readonly Dictionary<Type, string> templateCache = new();

        /// <summary>
        ///     Reset the passed scriptableObject to its default values
        /// </summary>
        public static void ResetObject(ScriptableObject scriptableObject, bool cache = true)
        {
            var type = scriptableObject.GetType();

            if (!templateCache.TryGetValue(type, out var json))
            {
                var template = ScriptableObject.CreateInstance(type);
                json = JsonUtility.ToJson(template);
                EngineCallbacks.RemoveCallbacks(template);
                Object.DestroyImmediate(template);
            }
            if (cache)
            {
                templateCache.TryAdd(type, json);
            }

            JsonUtility.FromJsonOverwrite(json, scriptableObject);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(scriptableObject);
#endif
        }

        public static RuntimeGUID GetRuntimeGUID<T>(this T scriptableObject) where T : ScriptableObject
        {
#if UNITY_EDITOR
            var assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(scriptableObject));
            return new RuntimeGUID(assetGuid);
#endif
#pragma warning disable
            foreach (var (guid, value) in AssetRegistry.Registry)
            {
                if (value == scriptableObject)
                {
                    return guid;
                }
            }

            return default(RuntimeGUID);
        }
    }
}
