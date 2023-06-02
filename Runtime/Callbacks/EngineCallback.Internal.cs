using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public partial class EngineCallbacks
    {
        #region Update Callbacks

        public static int EngineState { get; private set; }
        public static bool IsQuitting { get; private set; }
        public static bool ManualInitializationCompleted { get; private set; }

        private const int DefaultCapacity = 16;

        private static readonly List<IOnUpdate> updateListener = new(DefaultCapacity);
        private static readonly List<IOnLateUpdate> lateUpdateListener = new(DefaultCapacity);
        private static readonly List<IOnFixedUpdate> fixedUpdateListener = new(DefaultCapacity);

        private static readonly List<UpdateDelegate> updateDelegates = new(DefaultCapacity);
        private static readonly List<LateUpdateDelegate> lateUpdateDelegates = new(DefaultCapacity);
        private static readonly List<FixedUpdateDelegate> fixedUpdateDelegates = new(DefaultCapacity);

        private static readonly List<IOnBeforeFirstSceneLoad> beforeSceneLoadListener = new(DefaultCapacity);
        private static readonly List<IOnQuit> quitListener = new(DefaultCapacity);
        private static readonly List<IOnAfterFirstSceneLoad> afterFirstSceneLoadListener = new();
        private static readonly List<IOnInitializationCompleted> initializationCompletedListener = new();
        private static readonly List<Action> beforeSceneLoadDelegates = new(DefaultCapacity);
        private static readonly List<Action> quitDelegates = new(DefaultCapacity);

        private static bool beforeSceneLoadCompleted;
        private static bool afterSceneLoadCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupUpdateCallbacks()
        {
            IsQuitting = false;
            RuntimeHook.Create(OnUpdate, OnLateUpdate, OnFixedUpdate, OnQuit);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
#if DEBUG
            foreach (var listener in beforeSceneLoadListener)
            {
                try
                {
                    listener.OnBeforeFirstSceneLoad();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in beforeSceneLoadDelegates)
            {
                try
                {
                    listener();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in beforeSceneLoadListener)
            {
                listener.OnBeforeFirstSceneLoad();
            }

            foreach (var listener in beforeSceneLoadDelegates)
            {
                listener();
            }
#endif
            beforeSceneLoadCompleted = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
#if DEBUG
            foreach (var listener in afterFirstSceneLoadListener)
            {
                try
                {
                    listener.OnAfterFirstSceneLoad();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in afterFirstSceneLoadListener)
            {
                listener.OnAfterFirstSceneLoad();
            }
#endif
            afterSceneLoadCompleted = true;
        }

        private static void OnQuit()
        {
            IsQuitting = true;
            beforeSceneLoadCompleted = false;
            afterSceneLoadCompleted = false;
#if DEBUG
            foreach (var listener in quitListener)
            {
                try
                {
                    listener.OnQuit();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in quitDelegates)
            {
                try
                {
                    listener();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in quitListener)
            {
                listener.OnQuit();
            }

            foreach (var listener in quitDelegates)
            {
                listener();
            }
#endif
        }

        private static void OnUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            foreach (var listener in updateListener)
            {
                try
                {
                    listener.OnUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in updateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            foreach (var listener in updateListener)
            {
                listener.OnUpdate(deltaTime);
            }

            foreach (var listener in updateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        private static void OnLateUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            foreach (var listener in lateUpdateListener)
            {
                try
                {
                    listener.OnLateUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in lateUpdateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            foreach (var listener in lateUpdateListener)
            {
                listener.OnLateUpdate(deltaTime);
            }

            foreach (var listener in lateUpdateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        private static void OnFixedUpdate()
        {
#if DEBUG
            var deltaTime = Time.fixedDeltaTime;
            foreach (var listener in fixedUpdateListener)
            {
                try
                {
                    listener.OnFixedUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in fixedUpdateDelegates)
            {
                try
                {
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.fixedDeltaTime;
            foreach (var listener in fixedUpdateListener)
            {
                listener.OnFixedUpdate(deltaTime);
            }

            foreach (var listener in fixedUpdateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        #endregion
    }
}