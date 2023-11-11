using JetBrains.Annotations;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Reflection;

namespace MobX.Utilities.Singleton
{
    [AddressablesGroup("Singletons")]
    public abstract class SingletonAsset<T> : ScriptableAsset where T : SingletonAsset<T>
    {
        public static T Singleton => singleton ??= Singletons.Resolve<T>();
        private static T singleton;

        [PublicAPI]
        public bool IsSingleton => Singletons.Exists<T>() && Singletons.Resolve<T>() == this;

        protected override void OnEnable()
        {
            base.OnEnable();
            singleton = (T) this;

            if (Singletons.Exists<T>() is false)
            {
                Singletons.Register(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (singleton == this)
            {
                singleton = null;
            }
        }
    }
}