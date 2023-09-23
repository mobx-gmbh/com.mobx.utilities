using MobX.Utilities.Registry;
using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject, IDisposable
    {
        [Conditional("UNITY_EDITOR")]
        public void Repaint()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected virtual void OnEnable()
        {
            Gameloop.Register(this);
            EngineCallbacks.AddCallbacks(this);

#if UNITY_EDITOR
            AssetRegistry.RegisterRuntimeAsset(this);
#endif
        }

        protected virtual void OnDisable()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDestroy()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
            GC.SuppressFinalize(this);
        }

        ~RuntimeAsset()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
        }
    }
}