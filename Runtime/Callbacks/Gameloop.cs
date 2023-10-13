using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Callbacks
{
    /// <summary>
    ///     Class handles custom gameloop specific callbacks as well as other gameloop relevant data.
    ///     This includes time scale, frame count, editor state, initialization callbacks and more.
    /// </summary>
    public partial class Gameloop
    {
        #region Callbacks

        /// <summary>
        ///     Register an object making it receive custom callbacks.
        /// </summary>
        /// <param name="target">The object to register</param>
        [Conditional("ENABLE_GAMELOOP_CALLBACKS")]
        public static void Register(Object target)
        {
            RegisterInternal(target);
        }

        /// <summary>
        ///     Unregister an object making it no longer receive custom callbacks.
        /// </summary>
        /// <param name="target">The object to unregister</param>
        [Conditional("ENABLE_GAMELOOP_CALLBACKS")]
        public static void Unregister(Object target)
        {
            UnregisterInternal(target);
        }

        /// <summary>
        ///     Raise a custom callback method.
        /// </summary>
        /// <param name="callback">The name of the callback method</param>
        [Conditional("ENABLE_GAMELOOP_CALLBACKS")]
        public static void RaiseCallback(string callback)
        {
            RaiseCallbackInternal(callback);
        }

        /// <summary>
        ///     Raise the initialization completed callback.
        /// </summary>
        [Conditional("ENABLE_GAMELOOP_CALLBACKS")]
        public static void RaiseInitializationCompleted()
        {
            RaiseInitializationCompletedInternal();
        }

        /// <summary>
        ///     Begin an asynchronous shutdown process.
        /// </summary>
        public static Task ShutdownAsync()
        {
#if ENABLE_GAMELOOP_CALLBACKS
            return ShutdownAsyncInternal();
#else
            return Task.CompletedTask;
#endif
        }

        #endregion


        #region State

        /// <summary>
        ///     Returns true if the application quitting process has started.
        /// </summary>
        public static bool IsQuitting { get; private set; }

        /// <summary>
        ///     The editor play mode state. Will only return a valid value in editor.
        /// </summary>
        public static int EditorState { get; private set; }

        /// <summary>
        ///     Returns true if <see cref="RaiseInitializationCompleted" /> was raised.
        /// </summary>
        public static bool InitializationCompletedState { get; private set; }

        /// <summary>
        ///     Returns true if the before scene load callback was raised.
        /// </summary>
        public static bool BeforeSceneLoadCompleted { get; private set; }

        /// <summary>
        ///     Returns true if the after scene load callback was raised.
        /// </summary>
        public static bool AfterSceneLoadCompleted { get; private set; }

        /// <summary>
        ///     Get the current gameloop segment. (Update, LateUpdate etc.)
        /// </summary>
        public static Segment Segment { get; private set; } = Segment.None;

        /// <summary>
        ///     Get the current update count.
        /// </summary>
        public static int FrameCount { get; private set; }

        /// <summary>
        ///     Get the current physics update count.
        /// </summary>
        public static int FixedUpdateCount { get; private set; }

        #endregion


        #region Events

        /// <summary>
        ///     Called before the first scene is loaded.
        ///     This event is called retroactively.
        /// </summary>
        public static event Action BeforeFirstSceneLoad
        {
            add => beforeFirstSceneLoadCallbacks.AddNullChecked(value);
            remove => beforeFirstSceneLoadCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called after the first scene is loaded.
        ///     This event is called retroactively.
        /// </summary>
        public static event Action AfterFirstSceneLoad
        {
            add => afterFirstSceneLoadCallbacks.AddNullChecked(value);
            remove => afterFirstSceneLoadCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called during shutdown.
        /// </summary>
        public static event Action ApplicationQuit
        {
            add => applicationQuitCallbacks.AddNullChecked(value);
            remove => applicationQuitCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called when the focus of the application was changed.
        /// </summary>
        public static event Action<bool> ApplicationFocusChanged
        {
            add => applicationFocusCallbacks.AddNullChecked(value);
            remove => applicationFocusCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called when pause state of the application was changed.
        /// </summary>
        public static event Action<bool> ApplicationPauseChanged
        {
            add => applicationPauseCallbacks.AddNullChecked(value);
            remove => applicationPauseCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called every frame.
        /// </summary>
        public static event Action Update
        {
            add => updateCallbacks.AddNullChecked(value);
            remove => updateCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called every frame during late update.
        /// </summary>
        public static event Action LateUpdate
        {
            add => lateUpdateCallbacks.AddNullChecked(value);
            remove => lateUpdateCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called every physics update.
        /// </summary>
        public static event Action FixedUpdate
        {
            add => fixedUpdateCallbacks.AddNullChecked(value);
            remove => fixedUpdateCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called once every second.
        /// </summary>
        public static event Action SecondUpdate
        {
            add => secondUpdateCallbacks.AddNullChecked(value);
            remove => secondUpdateCallbacks.Remove(value);
        }

        /// <summary>
        ///     Called when asynchronous initialization has completed.
        ///     Requires <see cref="RaiseInitializationCompleted" /> to be called.
        ///     This event is called retroactively.
        /// </summary>
        public static event Action InitializationCompleted
        {
            add => initializationCompletedCallbacks.AddNullChecked(value);
            remove => initializationCompletedCallbacks.Remove(value);
        }

        #endregion


        #region Time Scale

        /// <summary>
        ///     A custom timescale implementation that can be modified by multiple sources.
        /// </summary>
        public static float TimeScale => CalculateTimeScale();

        /// <summary>
        ///     Add a custom timescale modification source.
        /// </summary>
        public static void AddTimeScaleModifier(Func<float> modifier)
        {
            timeScaleModifier.AddUnique(modifier);
        }

        /// <summary>
        ///     Remove a custom timescale modification source.
        /// </summary>
        public static void RemoveTimeScaleModifier(Func<float> modifier)
        {
            timeScaleModifier.Remove(modifier);
        }

        /// <summary>
        ///     Removes all active time scale modification sources.
        /// </summary>
        public static void ClearTimeScaleModifier()
        {
            timeScaleModifier.Clear();
        }

        #endregion


        #region Coroutines

        public static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (monoBehaviour == null)
            {
                return null;
            }
            return monoBehaviour.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (monoBehaviour == null)
            {
                return;
            }
            monoBehaviour.StopCoroutine(coroutine);
        }

        #endregion
    }
}
