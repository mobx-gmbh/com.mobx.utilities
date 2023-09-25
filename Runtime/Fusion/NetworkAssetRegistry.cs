using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using MobX.Utilities.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MobX.Utilities.Fusion
{
    public class NetworkAssetRegistry : SingletonAsset<NetworkAssetRegistry>
    {
        [ReadonlyInspector]
        [SerializeField] private List<ScriptableObject> networkAssets;

        public static T Resolve<T>(int networkAssetId) where T : ScriptableObject, INetworkAsset
        {
            if (Singleton.networkAssets.TryGetElementAt(networkAssetId, out var scriptableObject))
            {
                var asset = scriptableObject as T;
                Assert.IsNotNull(asset);
                Assert.IsTrue(asset.NetworkAssetID == networkAssetId);
                return asset;
            }
            Debug.LogError(nameof(NetworkAssetRegistry), $"Network Asset for id: [{networkAssetId}] was not found!");
            return null;
        }


        #region Editor

#if UNITY_EDITOR

        internal static void Register<T>(T asset) where T : ScriptableObject, INetworkAsset
        {
            Singleton.networkAssets.AddUnique(asset);
            var networkAssetID = Singleton.networkAssets.IndexOf(asset);
            asset.NetworkAssetID = networkAssetID;
        }

        static NetworkAssetRegistry()
        {
            Gameloop.BeforeDeleteAsset += OnBeforeDeleteAsset;
        }

        private static void OnBeforeDeleteAsset(string assetPath, Object asset)
        {
            if (asset is not INetworkAsset networkAsset)
            {
                return;
            }
            Singleton.networkAssets.Remove((ScriptableObject) networkAsset);
            UpdateAssetIndex();
            UnityEditor.EditorUtility.SetDirty(Singleton);
            UnityEditor.AssetDatabase.Refresh();
        }

        [Button]
        private static void UpdateAssetIndex()
        {
            Singleton.networkAssets.RemoveNull();

            for (var index = 0; index < Singleton.networkAssets.Count; index++)
            {
                var asset = Singleton.networkAssets[index];
                var networkAsset = (INetworkAsset) asset;
                networkAsset.NetworkAssetID = index;
            }
        }

#endif

        #endregion
    }
}