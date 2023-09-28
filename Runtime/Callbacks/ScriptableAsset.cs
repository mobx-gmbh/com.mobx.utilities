using MobX.Utilities.Unity;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    /// <summary>
    ///     Abstract base class for <see cref="ScriptableObject" />s that can receive <see cref="Gameloop" /> callbacks.
    ///     Use the <see cref="CallbackMethodAttribute" /> to receive custom callbacks on a target method.
    /// </summary>
    public abstract class ScriptableAsset : ScriptableObject
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
        }

        protected virtual void OnDisable()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
        }

        protected virtual void OnDestroy()
        {
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
        }

        /// <summary>
        ///     Reset the asset to its default values.
        /// </summary>
        public void ResetAsset()
        {
            ScriptableObjectUtility.ResetObject(this);
        }
    }
}
