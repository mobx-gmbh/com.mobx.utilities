using System;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public partial class Gameloop
    {
        static Gameloop()
        {
            Application.quitting += () => IsQuitting = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.update += OnEditorUpdate;
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstSceneLoaded()
        {
            Segment = Segment.BeforeFirstSceneLoad;
            IsQuitting = false;
#if ENABLE_GAMELOOP_CALLBACKS
            RuntimeMonoBehaviourEvents.Create(
                OnUpdate,
                OnLateUpdate,
                OnFixedUpdate,
                OnQuit,
                OnApplicationFocus,
                OnApplicationPause);
#endif
            for (var index = beforeFirstSceneLoadCallbacks.Count - 1; index >= 0; index--)
            {
                beforeFirstSceneLoadCallbacks[index]();
            }
            BeforeSceneLoadCompleted = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterFirstSceneLoaded()
        {
            Segment = Segment.AfterFirstSceneLoad;
            for (var index = afterFirstSceneLoadCallbacks.Count - 1; index >= 0; index--)
            {
                afterFirstSceneLoadCallbacks[index]();
            }
            AfterSceneLoadCompleted = true;
        }

        private static void OnUpdate()
        {
            Segment = Segment.Update;
#if DEBUG
            for (var index = updateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    updateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = updateCallbacks.Count - 1; index >= 0; index--)
            {
                updateCallbacks[index]();
            }
#endif
        }

        private static void OnLateUpdate()
        {
            Segment = Segment.LateUpdate;
#if DEBUG
            for (var index = lateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    lateUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = lateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                lateUpdateCallbacks[index]();
            }
#endif
            FrameCount++;
        }

        private static void OnFixedUpdate()
        {
            Segment = Segment.FixedUpdate;
#if DEBUG
            for (var index = fixedUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    fixedUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = fixedUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                fixedUpdateCallbacks[index]();
            }
#endif
            FixedUpdateCount++;
        }

        private static void OnQuit()
        {
            Segment = Segment.ApplicationQuit;
            IsQuitting = true;
#if DEBUG
            for (var index = applicationQuitCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationQuitCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = applicationQuitCallbacks.Count - 1; index >= 0; index--)
            {
                applicationQuitCallbacks[index]();
            }
#endif
        }

        private static void OnApplicationFocus(bool hasFocus)
        {
            Segment = Segment.ApplicationFocus;
#if DEBUG
            for (var index = applicationFocusCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationFocusCallbacks[index](hasFocus);
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = applicationFocusCallbacks.Count - 1; index >= 0; index--)
            {
                applicationFocusCallbacks[index](hasFocus);
            }
#endif
        }

        private static void OnApplicationPause(bool pauseState)
        {
            Segment = Segment.ApplicationPause;
#if DEBUG
            for (var index = applicationPauseCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    applicationPauseCallbacks[index](pauseState);
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = applicationPauseCallbacks.Count - 1; index >= 0; index--)
            {
                applicationPauseCallbacks[index](pauseState);
            }
#endif
        }

        private static void RaiseInitializationCompletedInternal()
        {
            Segment = Segment.InitializationCompleted;
            if (InitializationCompletedState)
            {
                Debug.LogWarning(logCategory, $"{nameof(RaiseInitializationCompleted)} has already been invoked!");
                return;
            }

            InitializationCompletedState = true;

#if DEBUG
            for (var index = initializationCompletedCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    initializationCompletedCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = initializationCompletedCallbacks.Count - 1; index >= 0; index--)
            {
                initializationCompletedCallbacks[index]();
            }
#endif
        }

        private static void RaiseCallbackInternal(string callbackName)
        {
            if (!customCallbacks.TryGetValue(callbackName, out var callbacks))
            {
                return;
            }
#if DEBUG
            for (var index = callbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    callbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = callbacks.Count - 1; index >= 0; index--)
            {
                callbacks[index]();
            }
#endif
        }

#if UNITY_EDITOR

        private static void OnEditorUpdate()
        {
            Segment = Segment.EditorUpdate;
#if DEBUG
            for (var index = editorUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                try
                {
                    editorUpdateCallbacks[index]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(logCategory, exception);
                }
            }
#else
            for (var index = editorUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                editorUpdateCallbacks[index]();
            }
#endif
        }

        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            EditorState = (int) state;
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
            for (var i = 0; i < exitPlayModeDelegate.Count; i++)
            {
                if (exitPlayModeDelegate[i] == null)
                {
                    continue;
                }
                try
                {
                    exitPlayModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void OnEnterPlayMode()
        {
            for (var i = 0; i < enterPlayModeDelegate.Count; i++)
            {
                if (enterPlayModeDelegate[i] == null)
                {
                    continue;
                }
                try
                {
                    enterPlayModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static void OnExitEditMode()
        {
            for (var i = 0; i < exitEditModeDelegate.Count; i++)
            {
                if (exitEditModeDelegate[i] == null)
                {
                    continue;
                }
                try
                {
                    exitEditModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            FrameCount = 0;
            FixedUpdateCount = 0;
        }

        private static void OnEnterEditMode()
        {
            IsQuitting = false;
            BeforeSceneLoadCompleted = false;
            AfterSceneLoadCompleted = false;
            InitializationCompletedState = false;

            for (var i = 0; i < enterEditModeDelegate.Count; i++)
            {
                if (enterEditModeDelegate[i] == null)
                {
                    continue;
                }
                try
                {
                    enterEditModeDelegate[i]();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            FrameCount = 0;
            FixedUpdateCount = 0;
        }

#endif
    }
}