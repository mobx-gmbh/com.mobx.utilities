using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject
    {
        private void OnEnable()
        {
            EngineCallbacks.AddCallbacks(this);
            OnAssetEnabled();
        }

        private void OnDisable()
        {
            EngineCallbacks.RemoveCallbacks(this);
            OnAssetDisabled();
        }

        protected virtual void OnAssetEnabled()
        {
        }

        protected virtual void OnAssetDisabled()
        {
        }
    }
}