#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Callbacks
{
    public sealed partial class EngineCallbacks : UnityEditor.AssetModificationProcessor
    {
        #region Asset Handling

        private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath,
            UnityEditor.RemoveAssetOptions options)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (asset is not ICallbackInterface)
            {
                return UnityEditor.AssetDeleteResult.DidNotDelete;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitPlayMode exitPlayModeCallback)
            {
                exitPlayModeListener.Remove(exitPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterPlayMode enterPlayModeCallback)
            {
                enterPlayModeListener.Remove(enterPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitEditMode exitEditModeCallback)
            {
                exitEditModeListener.Remove(exitEditModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterEditMode enterEditModeCallback)
            {
                enterEditModeListener.Remove(enterEditModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnAfterLoad afterLoadCallback)
            {
                afterLoadListener.Remove(afterLoadCallback);
            }

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion


        #region Fields

        private static readonly List<Action> exitPlayModeDelegate = new();
        private static readonly List<Action> enterPlayModeDelegate = new();
        private static readonly List<Action> exitEditModeDelegate = new();
        private static readonly List<Action> enterEditModeDelegate = new();

        private static readonly List<IOnExitPlayMode> exitPlayModeListener = new();
        private static readonly List<IOnEnterPlayMode> enterPlayModeListener = new();
        private static readonly List<IOnExitEditMode> exitEditModeListener = new();
        private static readonly List<IOnEnterEditMode> enterEditModeListener = new();

        #endregion


        #region Internal Subscribtions

        private static void AddOnExitPlayInternal(IOnExitPlayMode listener)
        {
            exitPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitPlayInternal(IOnExitPlayMode listener)
        {
            exitPlayModeListener.Remove(listener);
        }

        private static void AddOnEnterPlayInternal(IOnEnterPlayMode listener)
        {
            enterPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterPlayInternal(IOnEnterPlayMode listener)
        {
            enterPlayModeListener.Remove(listener);
        }

        private static void AddOnExitEditInternal(IOnExitEditMode listener)
        {
            exitEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitEditInternal(IOnExitEditMode listener)
        {
            exitEditModeListener.Remove(listener);
        }

        private static void AddOnEnterEditInternal(IOnEnterEditMode listener)
        {
            enterEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterEditInternal(IOnEnterEditMode listener)
        {
            enterEditModeListener.Remove(listener);
        }

        #endregion


        #region Play Mode State Changed

        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode();
                    break;
                case UnityEditor.PlayModeStateChange.ExitingEditMode:
                    OnExitEditMode();

                    break;
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode();

                    break;
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void OnExitPlayMode()
        {
            for (var i = 0; i < exitPlayModeListener.Count; i++)
            {
                exitPlayModeListener[i].OnExitPlayMode();
            }

            for (var i = 0; i < exitPlayModeDelegate.Count; i++)
            {
                exitPlayModeDelegate[i]();
            }
        }

        private static void OnEnterPlayMode()
        {
            for (var i = 0; i < enterPlayModeListener.Count; i++)
            {
                enterPlayModeListener[i].OnEnterPlayMode();
            }

            for (var i = 0; i < enterPlayModeDelegate.Count; i++)
            {
                enterPlayModeDelegate[i]();
            }
        }

        private static void OnExitEditMode()
        {
            for (var i = 0; i < exitEditModeListener.Count; i++)
            {
                exitEditModeListener[i].OnExitEditMode();
            }

            for (var i = 0; i < exitEditModeDelegate.Count; i++)
            {
                exitEditModeDelegate[i]();
            }
        }

        private static void OnEnterEditMode()
        {
            for (var i = 0; i < enterEditModeListener.Count; i++)
            {
                enterEditModeListener[i].OnEnterEditMode();
            }

            for (var i = 0; i < enterEditModeDelegate.Count; i++)
            {
                enterEditModeDelegate[i]();
            }
        }

        #endregion
    }
}
#endif