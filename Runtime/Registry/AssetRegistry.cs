using MobX.Utilities.Callbacks;
using MobX.Utilities.Collections;
using MobX.Utilities.Inspector;
using MobX.Utilities.Singleton;
using MobX.Utilities.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace MobX.Utilities.Registry
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public class AssetRegistry : SingletonAsset<AssetRegistry>
    {
        #region Inspector & Properties

        [Preserve]
        [SerializeField] private Map<string, Object> registry = new();

        public static IReadOnlyDictionary<string, Object> Registry => Singleton.registry;

        #endregion


        #region Public

        /// <summary>
        ///     Register a unique asset. Registered assets can be resolved using their <see cref="RuntimeGUID" />
        /// </summary>
        public static void Register<T>(T asset) where T : Object, IUniqueAsset
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => { Singleton.registry.Update(asset.GUID.Value, asset); };
#endif
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID
        /// </summary>
        public static T Resolve<T>(string guid) where T : Object
        {
            return (T) Singleton.registry[guid];
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID as a UnityEngine.Object
        /// </summary>
        public static Object ResolveObject(string guid)
        {
            return Singleton.registry[guid];
        }

        /// <summary>
        ///     Try Resolve an asset of type T by its GUID
        /// </summary>
        public static bool TryResolve<T>(string guid, out T result) where T : Object
        {
            if (Singleton.registry.TryGetValue(guid, out var instance))
            {
                result = instance as T;
                return result != null;
            }

            result = default(T);
            return false;
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [Button]
        [Foldout("Validation", false)]
        public bool ContainsAsset(Object obj)
        {
            return registry.ContainsValue(obj);
        }

        [Button]
        [Foldout("Validation", false)]
        public void Validate()
        {
            var assets = registry.ToArray();
            foreach (var (key, value) in assets)
            {
                if (value == null || value is not IUniqueAsset)
                {
                    Debug.Log("Asset Registry", "Removing invalid unique asset registry entry!");
                    registry.Remove(key);
                }
            }
        }

        [Button]
        private void ClearAll()
        {
            registry.Clear();
        }

        static AssetRegistry()
        {
            Gameloop.BeforeDeleteAsset += OnBeforeDeleteAsset;
        }

        private static void OnBeforeDeleteAsset(string assetPath, Object asset)
        {
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            Singleton.registry.TryRemove(guid);
        }

#endif

        #endregion
    }
}
