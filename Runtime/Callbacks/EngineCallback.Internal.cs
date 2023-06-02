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

        private const int DefaultCapacity = 16;

        private static readonly List<IOnUpdate> updateCallbacks = new(DefaultCapacity);
        private static readonly List<IOnLateUpdate> lateUpdateCallbacks = new(DefaultCapacity);
        private static readonly List<IOnFixedUpdate> fixedUpdateCallbacks = new(DefaultCapacity);

        private static readonly List<UpdateDelegate> updateDelegates = new(DefaultCapacity);
        private static readonly List<LateUpdateDelegate> lateUpdateDelegates = new(DefaultCapacity);
        private static readonly List<FixedUpdateDelegate> fixedUpdateDelegates = new(DefaultCapacity);

        private static readonly List<IOnLoad> beginPlayCallbacks = new(DefaultCapacity);
        private static readonly List<IOnQuit> quitCallbacks = new(DefaultCapacity);
        private static readonly List<IOnAwake> afterLoadListener = new();
        private static readonly List<Action> beginPlayDelegates = new(DefaultCapacity);
        private static readonly List<Action> quitDelegates = new(DefaultCapacity);

        private static bool beforeSceneLoadCompleted;
        private static bool afterSceneLoadCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
#if DEBUG
            foreach (var listener in beginPlayCallbacks)
            {
                try
                {
                    listener.OnBeginPlay();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in beginPlayDelegates)
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
            foreach (var listener in beginPlayCallbacks)
            {
                listener.OnBeginPlay();
            }

            foreach (var listener in beginPlayDelegates)
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
            foreach (var listener in afterLoadListener)
            {
                try
                {
                    listener.OnAwake();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in afterLoadListener)
            {
                listener.OnAwake();
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
            foreach (var listener in quitCallbacks)
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
            foreach (var listener in quitCallbacks)
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
            foreach (var listener in updateCallbacks)
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
            foreach (var listener in updateCallbacks)
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
            foreach (var listener in lateUpdateCallbacks)
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
            foreach (var listener in lateUpdateCallbacks)
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
            foreach (var listener in fixedUpdateCallbacks)
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
            foreach (var listener in fixedUpdateCallbacks)
            {
                listener.OnFixedUpdate(deltaTime);
            }

            foreach (var listener in fixedUpdateDelegates)
            {
                listener(deltaTime);
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupUpdateCallbacks()
        {
            IsQuitting = false;
            RuntimeHook.Create(OnUpdate, OnLateUpdate, OnFixedUpdate, OnQuit);
        }

        #endregion
    }
}