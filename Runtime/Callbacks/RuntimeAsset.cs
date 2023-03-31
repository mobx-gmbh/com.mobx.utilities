using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject
    {
        private void OnEnable()
        {
            EngineCallbacks.AddCallbacks(this);
            OnEnabled();
        }

        private void OnDisable()
        {
            EngineCallbacks.RemoveCallbacks(this);
            OnDisabled();
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }
    }
}
