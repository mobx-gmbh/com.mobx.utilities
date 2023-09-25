using UnityEngine;

namespace MobX.Utilities.Fusion
{
    public struct NetworkAssetReference<T> where T : ScriptableObject, INetworkAsset
    {
        private int _networkAssetId;

        public T ToAsset()
        {
            return NetworkAssetRegistry.Resolve<T>(_networkAssetId);
        }
    }
}