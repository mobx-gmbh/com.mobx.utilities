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

            if (listener is IOnLoad onBeginPlay)
            {
                AddOnLoadListener(onBeginPlay);
            }

            if (listener is IOnQuit onEndPlay)
            {
                AddOnQuitListener(onEndPlay);
            }

            if (listener is IOnAwake onAfterLoad)
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

            if (listener is IOnLoad onBeginPlay)
            {
                RemoveBeginPlayListener(onBeginPlay);
            }

            if (listener is IOnQuit onEndPlay)
            {
                RemoveQuitListener(onEndPlay);
            }

            if (listener is IOnAwake onAfterLoad)
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

        public static void AddOnLoadListener<T>(T listener) where T : class, IOnLoad
        {
            if (beforeSceneLoadCompleted)
            {
                listener.OnBeginPlay();
            }

            beginPlayCallbacks.AddUnique(listener, true);
        }

        public static void RemoveBeginPlayListener<T>(T listener) where T : class, IOnLoad
        {
            beginPlayCallbacks.Remove(listener);
        }

        public static void AddOnQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitCallbacks.AddUnique(listener, true);
        }

        public static void RemoveQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitCallbacks.Remove(listener);
        }

        public static void AddAfterLoadListener<T>(T listener) where T : class, IOnAwake
        {
            if (afterSceneLoadCompleted)
            {
                listener.OnAwake();
            }
            afterLoadListener.AddUnique(listener, true);
        }

        public static void RemoveAfterLoadListener<T>(T listener) where T : class, IOnAwake
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
