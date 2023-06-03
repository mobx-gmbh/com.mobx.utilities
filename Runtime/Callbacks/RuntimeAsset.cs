using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject, IDisposable
    {
        protected RuntimeAsset()
        {
            EngineCallbacks.AddCallbacks(this);
        }

        [Conditional("UNITY_EDITOR")]
        protected void Repaint()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public virtual void Dispose()
        {
            EngineCallbacks.RemoveCallbacks(this);
        }
    }
}