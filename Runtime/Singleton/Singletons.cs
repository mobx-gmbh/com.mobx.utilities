using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Singleton
{
    public sealed class Singletons : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Data

        public static bool IsLoaded => singleton != null;

#pragma warning disable 414
        private bool _enableEditing;
#pragma warning restore

        [ConditionalShow(nameof(_enableEditing), ReadOnly = true)]
        [ListOptions]
        [SerializeField] private List<Object> registry;

        [Button]
        [SpaceBefore]
        [Tooltip("Remove null objects from the registry")]
        private void ClearInvalid()
        {
            registry.RemoveNull();
        }

        [Button]
        [ConditionalShow(nameof(_enableEditing), false)]
        private void EnableEdit()
        {
            _enableEditing = true;
        }

        [Button]
        [ConditionalShow(nameof(_enableEditing))]
        private void DisableEdit()
        {
            _enableEditing = false;
        }

        #endregion


        #region Public

        public static void Register<T>(T instance) where T : Object
        {
#if UNITY_EDITOR
            async static Task WaitWhile(Func<bool> condition)
            {
                while (condition())
                {
                    await Task.Delay(25);
                }
            }

            static bool IsImport() => UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating;

            if (IsImport())
            {
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                WaitWhile(IsImport).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    Register(instance);
                }, scheduler);
                return;
            }
#endif
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
                    return (T)Singleton.registry[i];
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

        private static Singletons singleton;

        public static Singletons Singleton
        {
            get
            {
                // In the editor we load the singleton from the asset database.
#if UNITY_EDITOR
                if (singleton != null)
                {
                    return singleton;
                }

                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(Singletons)}");
                for (var i = 0; i < guids.Length; i++)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    singleton = UnityEditor.AssetDatabase.LoadAssetAtPath<Singletons>(path);
                    if (singleton != null)
                    {
                        break;
                    }
                }
#endif
                return singleton;
            }
        }

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
