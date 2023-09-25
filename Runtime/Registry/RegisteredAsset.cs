using MobX.Utilities.Callbacks;
using MobX.Utilities.Types;
using MobX.Utilities.Unity;
using UnityEngine;

namespace MobX.Utilities.Registry
{
    public class RegisteredAsset : ScriptableAsset, IUniqueAsset
    {
        [SerializeField] private RuntimeGUID guid;
        public RuntimeGUID GUID => guid;

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            guid = this.GetRuntimeGUID();
            AssetRegistry.Register(this);
#endif
        }
    }
}