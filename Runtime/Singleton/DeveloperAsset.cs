using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System;

namespace MobX.Utilities.Singleton
{
    /// <summary>
    ///     Base type for developer specific configuration files.
    /// </summary>
    public abstract class DeveloperAsset<T> : ScriptableAsset where T : DeveloperAsset<T>
    {
        private static T local;

        /// <summary>
        ///     The locally saved/applied configuration file.
        /// </summary>
        public static T LocalInstance
        {
            get
            {
#if !UNITY_EDITOR
                return Singletons.Resolve<T>();
#else
                if (local == null)
                {
                    var guid = UnityEditor.EditorPrefs.GetString(typeof(T).FullName);
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

                    LocalInstance = asset;
                }

                if (local == null)
                {
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Settings");
                    }

                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings/Developer"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets/Settings", "Developer");
                    }

                    var asset = CreateInstance<T>();
                    var assetPath = $"Assets/Settings/Developer/{typeof(T).Name}.{Environment.UserName.Trim()}.asset";
                    UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log("Singleton", $"Creating new {typeof(T).Name} instance at {assetPath}!");
                    LocalInstance = asset;
                }

                if (local == null)
                {
                    LocalInstance = Singletons.Resolve<T>();
                }

                return local;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                if (value == null)
                {
                    return;
                }

                var path = UnityEditor.AssetDatabase.GetAssetPath(value);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString(typeof(T).FullName, guid);
                if (Singletons.Exists<T>() is false)
                {
                    Singletons.Register(value);
                }
#endif
                local = value;
            }
        }

#if UNITY_EDITOR

        private bool IsLocal()
        {
            return this == LocalInstance;
        }

        private bool IsGlobal()
        {
            return this == Singletons.Resolve<T>();
        }

        [Button]
        [Foldout("Configuration")]
        [ConditionalShow(nameof(IsLocal), false)]
        public void DeclareAsLocal()
        {
            LocalInstance = (T) this;
        }

        [Button]
        [Foldout("Configuration")]
        [ConditionalShow(nameof(IsGlobal), false)]
        public void DeclareAsGlobal()
        {
            Singletons.Register((T) this);
        }

#endif
    }
}