using MobX.Utilities.Inspector;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Singleton
{
    public sealed class SingletonRegistry : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Data

#pragma warning disable 414
        private bool _enableEditing;
#pragma warning restore

        [ConditionalShow(nameof(_enableEditing), ReadOnly = true)]
        [ListOptions]
        [SerializeField] private List<Object> registry;

        [Button]
        [SpaceBefore]
        [Tooltip("Remove null objects from the registry")]
        private void ClearInvalid() => registry.RemoveNull();

        [Button]
        [ConditionalShow(nameof(_enableEditing), false)]
        private void EnableEdit() => _enableEditing = true;

        [Button]
        [ConditionalShow(nameof(_enableEditing))]
        private void DisableEdit() => _enableEditing = false;

        #endregion


        #region Public

        public static async void Register<T>(T instance) where T : Object
        {
            if (await InitializeAsync())
            {
                RegisterInternal(instance);
            }
        }

        private static void RegisterInternal<T>(T instance) where T : Object
        {
            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                if (Singleton.registry[i].GetType() == typeof(T))
                {
                    Singleton.registry[i] = instance;
                    return;
                }
            }

            Singleton.registry.Add(instance);
        }

        public static T Resolve<T>() where T : Object
        {
            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                if (Singleton.registry[i].GetType() == typeof(T))
                {
                    return (T) Singleton.registry[i];
                }
            }

            Debug.LogError("Singleton", $"No singleton instance of type {typeof(T)} registered!");
            Debug.Log(Singleton.registry);
            return null;
        }

        public static bool Exists<T>()
        {
            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                if (Singleton.registry[i].GetType() == typeof(T))
                {
                    return Singleton.registry[i];
                }
            }

            return false;
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


        #region Serialization

        public void OnAfterDeserialize()
        {
            singleton = this;
        }

        public void OnBeforeSerialize()
        {
        }

        #endregion
    }
}