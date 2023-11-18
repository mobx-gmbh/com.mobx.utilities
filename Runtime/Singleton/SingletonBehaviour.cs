using JetBrains.Annotations;
using UnityEngine;

namespace MobX.Utilities.Singleton
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        /// <summary>
        ///     The current singleton instance.
        /// </summary>
        [PublicAPI]
        public static T Singleton { get; private set; }

        /// <summary>
        ///     True if a singleton instance exists.
        /// </summary>
        [PublicAPI]
        public static bool SingletonInitialized { get; private set; }

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Singleton", $"More that one instance of {typeof(T).Name} found!");
                return;
            }

            Singleton = (T) this;
            SingletonInitialized = true;
        }

        protected virtual void OnDestroy()
        {
            if (Singleton != this)
            {
                return;
            }

            Singleton = null;
            SingletonInitialized = false;
        }
    }
}