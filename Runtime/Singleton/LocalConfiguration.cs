using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System;

namespace MobX.Utilities.Singleton
{
    /// <summary>
    ///     Base type for developer specific configuration files.
    /// </summary>
    public abstract class LocalConfiguration<T> : ScriptableAsset where T : LocalConfiguration<T>
    {
        private static T local;

        /// <summary>
        ///     The locally saved/applied configuration file.
        /// </summary>
        public static T Local
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

                    Local = asset;
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
                    Local = asset;
                }

                if (local == null)
                {
                    Local = Singletons.Resolve<T>();
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
            return this == Local;
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
            Local = (T) this;
        }

        [Button]
        [Foldout("Configuration")]
        [ConditionalShow(nameof(IsGlobal), false)]
        public void DeclareAsGlobal()
        {
            Singletons.Register((T) this);
        }
#endif // UNITY_EDITOR
    }
}