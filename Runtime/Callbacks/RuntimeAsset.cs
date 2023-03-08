using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject
    {
        protected RuntimeAsset()
        {
            EngineCallbacks.AddCallbacks(this);
        }
    }
}