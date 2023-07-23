using MobX.Utilities.Callbacks;
using MobX.Utilities.Collections;
using MobX.Utilities.Inspector;
using MobX.Utilities.Singleton;
using MobX.Utilities.Types;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Registry
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public class AssetRegistry : SingletonAsset<AssetRegistry>
    {
        [Annotation("Don't modify this map manually!")]
        [SerializeField] private Map<string, Object> registry = new();

        public static IReadOnlyDictionary<string, Object> Registry => Singleton.registry;


        #region Public

        /// <summary>
        ///     Register a unique asset. Registered assets can be resolved using their <see cref="RuntimeGUID" />
        /// </summary>
        public static void Register<T>(T asset) where T : Object, IUniqueAsset
        {
            Singleton.registry.Update(asset.GUID.Value, asset);
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID
        /// </summary>
        public static T Resolve<T>(string guid) where T : Object
        {
            return (T) Singleton.registry[guid];
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID
        /// </summary>
        public static Object ResolveFast(string guid)
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

        static AssetRegistry()
        {
            EngineCallbacks.BeforeDeleteAsset += OnBeforeDeleteAsset;
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
