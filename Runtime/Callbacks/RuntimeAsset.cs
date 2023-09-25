using System;

namespace MobX.Utilities.Callbacks
{
    [Obsolete("Use ScriptableAsset instead!")]
    public abstract class RuntimeAsset : ScriptableAsset, IDisposable
    {
        public virtual void Dispose()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
        }
    }
}