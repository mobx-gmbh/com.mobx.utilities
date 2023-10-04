using MobX.Utilities.Callbacks;
using MobX.Utilities.Reflection;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Singleton
{
    [AddressablesGroup("Singletons")]
    public abstract class SingletonAsset<T> : ScriptableObject where T : SingletonAsset<T>
    {
        public static T Singleton => singleton ??= Singletons.Resolve<T>();
        private static T singleton;

        protected virtual void OnEnable()
        {
            singleton = (T) this;
            Gameloop.Register(this);
            EngineCallbacks.AddCallbacks(this);

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

        public bool IsSingleton => Singletons.Exists<T>() && Singletons.Resolve<T>() == this;

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