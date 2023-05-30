using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public partial class EngineCallbacks
    {
        #region Initialization

        static EngineCallbacks()
        {
            Application.quitting += () => IsQuitting = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
#endif
        }

        #endregion


        #region Callbacks

        public static void AddCallbacks<T>(T listener) where T : class
        {
            if (listener is IOnUpdate onUpdate)
            {
                AddUpdateListener(onUpdate);
            }

            if (listener is IOnLateUpdate onLateUpdate)
            {
                AddLateUpdateListener(onLateUpdate);
            }

            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                AddFixedUpdateListener(onFixedUpdate);
            }

            if (listener is IOnBeginPlay onBeginPlay)
            {
                AddBeginPlayListener(onBeginPlay);
            }

            if (listener is IOnEndPlay onEndPlay)
            {
                AddEndPlayListener(onEndPlay);
            }

            if (listener is IOnAfterLoad onAfterLoad)
            {
                AddAfterLoadListener(onAfterLoad);
            }

#if UNITY_EDITOR
            AddOnExitPlayInternal(listener as IOnExitPlayMode);
            AddOnEnterPlayInternal(listener as IOnEnterPlayMode);
            AddOnExitEditInternal(listener as IOnExitEditMode);
            AddOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        public static void RemoveCallbacks<T>(T listener) where T : class
        {
            if (listener is IOnUpdate onUpdate)
            {
                RemoveUpdateListener(onUpdate);
            }

            if (listener is IOnLateUpdate onLateUpdate)
            {
                RemoveLateUpdateListener(onLateUpdate);
            }

            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                RemoveFixedUpdateListener(onFixedUpdate);
            }

            if (listener is IOnBeginPlay onBeginPlay)
            {
                RemoveBeginPlayListener(onBeginPlay);
            }

            if (listener is IOnEndPlay onEndPlay)
            {
                RemoveEndPlayListener(onEndPlay);
            }

            if (listener is IOnAfterLoad onAfterLoad)
            {
                RemoveAfterLoadListener(onAfterLoad);
            }

#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener as IOnExitPlayMode);
            RemoveOnEnterPlayInternal(listener as IOnEnterPlayMode);
            RemoveOnExitEditInternal(listener as IOnExitEditMode);
            RemoveOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        #endregion


        #region Runtime Callbacks

        public static void AddBeginPlayListener<T>(T listener) where T : class, IOnBeginPlay
        {
            if (initialized)
            {
                listener.OnBeginPlay();
            }

            beginPlayCallbacks.AddUnique(listener, true);
        }

        public static void RemoveBeginPlayListener<T>(T listener) where T : class, IOnBeginPlay
        {
            beginPlayCallbacks.Remove(listener);
        }

        public static void AddEndPlayListener<T>(T listener) where T : class, IOnEndPlay
        {
            endPlayCallbacks.AddUnique(listener, true);
        }

        public static void RemoveEndPlayListener<T>(T listener) where T : class, IOnEndPlay
        {
            endPlayCallbacks.Remove(listener);
        }

        public static void AddAfterLoadListener<T>(T listener) where T : class, IOnAfterLoad
        {
            afterLoadListener.AddUnique(listener, true);
        }

        public static void RemoveAfterLoadListener<T>(T listener) where T : class, IOnAfterLoad
        {
            afterLoadListener.Remove(listener);
        }

        #endregion


        #region Update Callbacks

        public static void AddUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateCallbacks.AddUnique(listener, true);
        }

        public static void AddLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateCallbacks.AddUnique(listener, true);
        }

        public static void AddFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateCallbacks.AddUnique(listener, true);
        }

        public static void RemoveUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateCallbacks.Remove(listener);
        }

        public static void RemoveLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateCallbacks.Remove(listener);
        }

        public static void RemoveFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateCallbacks.Remove(listener);
        }

        #endregion


        #region Add State Callbacks

        [Conditional("UNITY_EDITOR")]
        public static void AddExitPlayModeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            AddOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterPlayModeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            AddOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            AddOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            AddOnEnterEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitPlaymodeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterPlaymodeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            RemoveOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            RemoveOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            RemoveOnEnterEditInternal(listener);
#endif
        }

        #endregion
    }
}