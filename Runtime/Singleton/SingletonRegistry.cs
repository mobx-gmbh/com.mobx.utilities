using MobX.Utilities.Collections;
using MobX.Utilities.Inspector;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Singleton
{
    public sealed class SingletonRegistry : ScriptableObject
    {
        #region Data

        [Annotation("Map contains references to singleton assets. Do not modify manually!")]
        [ShowInInspector] private bool _enableEditing;

        [ConditionalShow(nameof(_enableEditing), ReadOnly = true)]
        [SerializeField] private Map<string, Object> registryMap;

        #endregion


        #region Public

        public static async void Register<T>(T instance) where T : Object
        {
            if (await InitializeAsync())
            {
                RegisterInternal(instance);
            }
            else
            {
                Debug.Log(":(");
            }
        }

        private static void RegisterInternal<T>(T instance) where T : Object
        {
            Singleton.registryMap.AddOrUpdate(Key<T>(), instance);
        }

        public static T Resolve<T>() where T : Object
        {
            if (Singleton.registryMap.TryGetValue(Key<T>(), out var value))
            {
                return value as T;
            }

            Debug.LogError("Singleton", $"No singleton instance of type {typeof(T)} registered!");
            Debug.Log(Singleton.registryMap);
            return null;
        }

        public static bool Exists<T>()
        {
            if (!Singleton.registryMap.TryGetValue(Key<T>(), out var value))
            {
                return false;
            }

            return value != null;
        }

        #endregion


        #region Singleton

        private static SingletonRegistry singleton;

        public static SingletonRegistry Singleton
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (singleton == null)
                {
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.AssetDatabase.SaveAssets();
#endif
                    singleton = Resources.Load<SingletonRegistry>("Singletons");
                }

                if (singleton == null)
                {
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
                    singleton = UnityEditor.AssetDatabase.LoadAssetAtPath<SingletonRegistry>("Assets/Resources/Singletons.asset");
#endif
                    Assert.IsNotNull(singleton);
                }

                if (singleton == null)
                {
                    Debug.LogError("Singleton", "No singleton registry found! Please create a new registry!");
                }

                return singleton;
            }
        }

        private static async Task<bool> InitializeAsync()
        {
            var attempts = 0;
            do
            {
                if (singleton == null)
                {
                    singleton = Resources.Load<SingletonRegistry>("Singletons");
                }

                if (singleton == null)
                {
    #if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
                    singleton = UnityEditor.AssetDatabase.LoadAssetAtPath<SingletonRegistry>("Assets/Resources/Singletons.asset");
    #endif
                }

                if (singleton != null)
                {
                    return true;
                }

                await Task.Delay(25);
            }
            while (attempts++ < 10);

            return false;
        }

        #endregion


        #region Helper

        private static string Key<T>() => typeof(T).FullName!;

        #endregion
    }
}