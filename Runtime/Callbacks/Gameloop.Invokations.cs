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
            for (var index = updateCallbacks.Count - 1; index >= 0; index--)
            {
                updateCallbacks[index]();
            }
        }

        private static void OnLateUpdate()
        {
            Segment = Segment.LateUpdate;
            for (var index = lateUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                lateUpdateCallbacks[index]();
            }

            FrameCount++;
        }

        private static void OnFixedUpdate()
        {
            Segment = Segment.FixedUpdate;
            for (var index = fixedUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                fixedUpdateCallbacks[index]();
            }
        }

        private static void OnQuit()
        {
            Segment = Segment.ApplicationQuit;
            IsQuitting = true;
            for (var index = applicationQuitCallbacks.Count - 1; index >= 0; index--)
            {
                applicationQuitCallbacks[index]();
            }
        }

        private static void OnApplicationFocus(bool hasFocus)
        {
            Segment = Segment.ApplicationFocus;
            for (var index = applicationFocusCallbacks.Count - 1; index >= 0; index--)
            {
                applicationFocusCallbacks[index](hasFocus);
            }
        }

        private static void OnApplicationPause(bool pauseState)
        {
            Segment = Segment.ApplicationPause;
            for (var index = applicationPauseCallbacks.Count - 1; index >= 0; index--)
            {
                applicationPauseCallbacks[index](pauseState);
            }
        }

        private static void RaiseInitializationCompletedInternal()
        {
            Segment = Segment.InitializationCompleted;
            if (InitializationCompletedState)
            {
                Debug.LogWarning("Gameloop", $"{nameof(RaiseInitializationCompleted)} has already been invoked!");
                return;
            }

            InitializationCompletedState = true;

            for (var index = initializationCompletedCallbacks.Count - 1; index >= 0; index--)
            {
                initializationCompletedCallbacks[index]();
            }

            Debug.Log("Gameloop", "Initialization Completed");
        }

        private static void RaiseCallbackInternal(string callbackName)
        {
            if (customCallbacks.TryGetValue(callbackName, out var callbacks))
            {
                for (var index = callbacks.Count - 1; index >= 0; index--)
                {
                    callbacks[index]();
                }
            }
        }

#if UNITY_EDITOR

        private static void OnEditorUpdate()
        {
            Segment = Segment.EditorUpdate;
            for (var index = editorUpdateCallbacks.Count - 1; index >= 0; index--)
            {
                editorUpdateCallbacks[index]();
            }
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
                exitPlayModeDelegate[i]();
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
                enterPlayModeDelegate[i]();
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
                exitEditModeDelegate[i]();
            }

            FrameCount = 0;
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
                enterEditModeDelegate[i]();
            }

            FrameCount = 0;
        }

#endif
    }
}