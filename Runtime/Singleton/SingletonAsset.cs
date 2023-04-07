using MobX.Utilities.Callbacks;
using UnityEngine;

namespace MobX.Utilities.Singleton
{
    public abstract class SingletonAsset<T> : ScriptableObject where T : SingletonAsset<T>
    {
        public static T Singleton
        {
            get => Singletons.Resolve<T>();
            private set => Singletons.Register(value);
        }

        protected virtual void OnEnable()
        {
            EngineCallbacks.AddCallbacks(this);
            Singleton = (T) this;
        }

        private void OnDisable()
        {
            EngineCallbacks.RemoveCallbacks(this);
        }
    }
}