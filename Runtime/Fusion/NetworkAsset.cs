using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Fusion
{
    public abstract class NetworkAsset : ScriptableAsset, INetworkAsset
    {
        [ReadonlyInspector]
        [SerializeField] private int networkAssetID;

        int INetworkAsset.NetworkAssetID
        {
            get => networkAssetID;
            set => networkAssetID = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            NetworkAssetRegistry.Register(this);
#endif
        }
    }
}