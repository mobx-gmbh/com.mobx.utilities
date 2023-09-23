using JetBrains.Annotations;
using System;
using System.Diagnostics;
using UnityEngine;

// ReSharper disable RedundantAssignment

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

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddCallbacks<T>(T listener) where T : class
        {
            if (listener is not ICallbackInterface)
            {
                return;
            }

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

            if (listener is IOnQuit onQuit)
            {
                AddOnQuitListener(onQuit);
            }

            if (listener is IOnBeforeFirstSceneLoad onBeforeFirstSceneLoad)
            {
                AddBeforeFirstSceneLoadListener(onBeforeFirstSceneLoad);
            }

            if (listener is IOnAfterFirstSceneLoad onAfterFirstSceneLoad)
            {
                AddAfterFirstSceneLoadListener(onAfterFirstSceneLoad);
            }

            if (listener is IOnInitializationCompleted onInitializationCompleted)
            {
                AddInitializationCompletedListener(onInitializationCompleted);
            }

            if (listener is IOnApplicationFocusChanged onApplicationFocusChanged)
            {
                AddApplicationFocusChangedListener(onApplicationFocusChanged);
            }

            if (listener is IOnApplicationPause onApplicationPause)
            {
                AddApplicationPauseChangedListener(onApplicationPause);
            }

#if UNITY_EDITOR
            AddOnExitPlayInternal(listener as IOnExitPlayMode);
            AddOnEnterPlayInternal(listener as IOnEnterPlayMode);
            AddOnExitEditInternal(listener as IOnExitEditMode);
            AddOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveCallbacks<T>(T listener) where T : class
        {
            if (listener is not ICallbackInterface)
            {
                return;
            }

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

            if (listener is IOnQuit onQuit)
            {
                RemoveQuitListener(onQuit);
            }

            if (listener is IOnBeforeFirstSceneLoad onBeforeFirstSceneLoad)
            {
                RemoveBeforeFirstSceneLoadListener(onBeforeFirstSceneLoad);
            }

            if (listener is IOnAfterFirstSceneLoad onAfterFirstSceneLoad)
            {
                RemoveAfterFirstSceneLoadListener(onAfterFirstSceneLoad);
            }

            if (listener is IOnInitializationCompleted onInitializationCompleted)
            {
                RemoveInitializationCompletedListener(onInitializationCompleted);
            }

            if (listener is IOnApplicationFocusChanged onApplicationFocusChanged)
            {
                RemoveApplicationFocusChangedListener(onApplicationFocusChanged);
            }

            if (listener is IOnApplicationPause onApplicationPause)
            {
                RemoveApplicationPauseChangedListener(onApplicationPause);
            }

#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener as IOnExitPlayMode);
            RemoveOnEnterPlayInternal(listener as IOnEnterPlayMode);
            RemoveOnExitEditInternal(listener as IOnExitEditMode);
            RemoveOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        #endregion


        #region Before First Scene Loaded

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddBeforeFirstSceneLoadListener<T>(T listener) where T : class, IOnBeforeFirstSceneLoad
        {
            if (beforeSceneLoadCompleted)
            {
                listener.OnBeforeFirstSceneLoad();
            }

            beforeFirstSceneLoadListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveBeforeFirstSceneLoadListener<T>(T listener) where T : class, IOnBeforeFirstSceneLoad
        {
            beforeFirstSceneLoadListener.Remove(listener);
        }

        #endregion


        #region After First Scene Loaded

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddAfterFirstSceneLoadListener<T>(T listener) where T : class, IOnAfterFirstSceneLoad
        {
            if (afterSceneLoadCompleted)
            {
                listener.OnAfterFirstSceneLoad();
            }
            afterFirstSceneLoadListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveAfterFirstSceneLoadListener<T>(T listener) where T : class, IOnAfterFirstSceneLoad
        {
            afterFirstSceneLoadListener.Remove(listener);
        }

        #endregion


        #region On Quit

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddOnQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitListener.Remove(listener);
        }

        #endregion


        #region After Initialization

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void NotifyInitializationCompleted()
        {
            if (ManualInitializationCompleted)
            {
                Debug.Log("Engine Callbacks", $"{nameof(NotifyInitializationCompleted)} has already been invoked!");
                return;
            }

            ManualInitializationCompleted = true;

            foreach (var onInitializationCompleted in initializationCompletedListener)
            {
                try
                {
                    onInitializationCompleted.OnInitializationCompleted();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var initializationCompletedDelegate in initializationCompletedDelegates)
            {
                try
                {
                    initializationCompletedDelegate();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddInitializationCompletedListener<T>(T listener) where T : class, IOnInitializationCompleted
        {
            if (ManualInitializationCompleted)
            {
                listener.OnInitializationCompleted();
            }
            initializationCompletedListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveInitializationCompletedListener<T>(T listener)
            where T : class, IOnInitializationCompleted
        {
            initializationCompletedListener.Remove(listener);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddApplicationFocusChangedListener<T>(T listener)
            where T : class, IOnApplicationFocusChanged
        {
            focusListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveApplicationFocusChangedListener<T>(T listener)
            where T : class, IOnApplicationFocusChanged
        {
            focusListener.Remove(listener);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddApplicationPauseChangedListener<T>(T listener)
            where T : class, IOnApplicationPause
        {
            pauseListener.AddUnique(listener, true);
        }

        [PublicAPI]
        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveApplicationPauseChangedListener<T>(T listener)
            where T : class, IOnApplicationPause
        {
            pauseListener.Remove(listener);
        }

        #endregion


        #region Update Callbacks

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateListener.AddUnique(listener, true);
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateListener.AddUnique(listener, true);
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateListener.AddUnique(listener, true);
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateListener.Remove(listener);
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateListener.Remove(listener);
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateListener.Remove(listener);
        }

        #endregion


        #region Add State Callbacks

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddExitPlayModeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            AddOnExitPlayInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddEnterPlayModeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            AddOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            AddOnExitEditInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void AddEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            AddOnEnterEditInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveExitPlaymodeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveEnterPlaymodeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            RemoveOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            RemoveOnExitEditInternal(listener);
#endif
        }

        [Conditional("ENABLE_LEGACY_ENGINE_CALLBACKS")]
        public static void RemoveEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            RemoveOnEnterEditInternal(listener);
#endif
        }

        #endregion
    }
}