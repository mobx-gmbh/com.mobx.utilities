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
        [SerializeField] private List<Object> networkAssets;

        public static T Resolve<T>(int networkAssetId) where T : Object, INetworkAsset
        {
            if (Singleton.networkAssets.TryGetElementAt(networkAssetId, out var asset))
            {
                var networkAsset = asset as T;
                Assert.IsNotNull(networkAsset);
                Assert.IsTrue(networkAsset.NetworkAssetID == networkAssetId);
                return networkAsset;
            }
            Debug.LogError(nameof(NetworkAssetRegistry), $"Network Asset for id: [{networkAssetId}] was not found!");
            return null;
        }

        public static bool IsNull(int networkAssetId)
        {
            return !Singleton.networkAssets.TryGetElementAt(networkAssetId, out var asset) || asset == null;
        }

        public static string GetStringRepresentation(int networkAssetId)
        {
            return $"ID: ({networkAssetId}) Asset: ({(Singleton.networkAssets.TryGetElementAt(networkAssetId, out var asset) ? asset.name : "null")})";
        }

        public static void Register<T>(T asset) where T : Object, INetworkAsset
        {
            Singleton.networkAssets.AddUnique(asset);
            var networkAssetID = Singleton.networkAssets.IndexOf(asset);
            asset.NetworkAssetID = networkAssetID;
        }


        #region Editor

#if UNITY_EDITOR

        static NetworkAssetRegistry()
        {
            Gameloop.BeforeDeleteAsset += OnBeforeDeleteAsset;
        }

        private static void OnBeforeDeleteAsset(string assetPath, Object element)
        {
            if (element is not INetworkAsset networkAsset)
            {
                return;
            }
            Singleton.networkAssets.Remove((Object) networkAsset);
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
