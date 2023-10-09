using JetBrains.Annotations;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Reflection;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Singleton
{
    [AddressablesGroup("Singletons")]
    public abstract class SingletonAsset<T> : ScriptableObject where T : SingletonAsset<T>
    {
        [Tooltip("When enabled, this asset can receive custom callback methods")]
        [SerializeField] private bool receiveCallbacks = true;

        public static T Singleton => singleton ??= Singletons.Resolve<T>();
        private static T singleton;

        [PublicAPI]
        public bool IsSingleton => Singletons.Exists<T>() && Singletons.Resolve<T>() == this;

        protected virtual void OnEnable()
        {
            singleton = (T) this;
            if (receiveCallbacks)
            {
                Gameloop.Register(this);
                EngineCallbacks.AddCallbacks(this);
            }

            if (Singletons.Exists<T>() is false)
            {
                Singletons.Register(this);
            }
        }

        private void OnDisable()
        {
            if (singleton == this)
            {
                singleton = null;
            }
            Gameloop.Unregister(this);
            EngineCallbacks.RemoveCallbacks(this);
        }

        [Conditional("UNITY_EDITOR")]
        protected void Repaint()
        {
#if UNITY_EDITOR
            if (Gameloop.IsQuitting)
            {
                return;
            }
            if (this == null)
            {
                return;
            }
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}