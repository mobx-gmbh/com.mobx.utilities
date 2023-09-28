using MobX.Utilities.Callbacks;
using MobX.Utilities.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities
{
    /// <summary>
    ///     Networked State Machine Behaviour. Handles state callbacks & transitions.
    /// </summary>
    public abstract class StateMachine : MonoBehaviour
    {
        #region Type Definitions

        private enum TransitionType
        {
            None = 0,
            Timer = 1
        }

        private readonly struct ConditionalTransition
        {
            public readonly int FromState;
            public readonly int ToState;
            public readonly Func<bool> Condition;

            public ConditionalTransition(int fromState, int toState, Func<bool> condition)
            {
                FromState = fromState;
                ToState = toState;
                Condition = condition;
            }
        }

        private class TimedStateTransitions
        {
            public readonly int FromState;
            public readonly int ToState;
            public readonly float Duration;
            public Timer Timer;

            public TimedStateTransitions(int fromState, int toState, float duration)
            {
                FromState = fromState;
                ToState = toState;
                Duration = duration;
                Timer = Timer.None;
            }

            public void Start()
            {
                Timer = new Timer(Duration);
            }
        }

        #endregion


        #region State Settings

        // Constants

        private const int MaxStateBufferLength = 4;
        private const int MinStateBufferLength = 1;

        /// <summary>
        ///     Delay in seconds after which a state transition is ignored.
        /// </summary>
        protected float InvokeRetroactiveStateTransitionsAfter { get; set; } = 7f;

        /// <summary>
        ///     Get / Set the amount of buffered state transitions that will be invoked retroactively when changing states.
        /// </summary>
        protected int StateBufferLength
        {
            get => _stateBufferLength;
            set => _stateBufferLength = Mathf.Clamp(value, MinStateBufferLength, MaxStateBufferLength);
        }

        /// <summary>
        ///     Get the timer of an active transition.
        /// </summary>
        protected float GetTransitionCountdown => StateTransitionTimer.RemainingTime.GetValueOrDefault();

        /// <summary>
        ///     Underlying primitive state representation
        /// </summary>
        protected int PrimitiveState => StateBuffer[0].State;

        private int _defaultState;

        private struct Buffer
        {
            public readonly int State;
            public readonly float TimeStamp;

            public Buffer(int state, float timeStamp)
            {
                State = state;
                TimeStamp = timeStamp;
            }
        }

        private Buffer[] StateBuffer { get; } = new Buffer[4];

        /// <summary>
        ///     When enabled, the state machine is automatically updated every network tick.
        ///     You can call <see cref="UpdateStateMachine" /> to manually invoke a state machine update on the state authority.
        /// </summary>
        protected bool StateMachineUpdateEnabled
        {
            get => _stateMachineUpdateEnabled;
            set
            {
                if (_stateMachineUpdateEnabled is false && value)
                {
                    Gameloop.Update += UpdateStateMachine;
                    _stateMachineUpdateEnabled = true;
                    return;
                }
                if (_stateMachineUpdateEnabled && value is false)
                {
                    Gameloop.Update -= UpdateStateMachine;
                    _stateMachineUpdateEnabled = false;
                }
            }
        }

        private bool _stateMachineUpdateEnabled;

        private int QueuedState { get; set; }
        private Timer StateTransitionTimer { get; set; } = Timer.None;
        private TransitionType Transition { get; set; } = TransitionType.None;

        private int StateChangeCount { get; set; }

        // Transitions & callbacks

        private readonly Dictionary<int, List<Action>> _stateEnterCallbacks = new();
        private readonly Dictionary<int, List<Action>> _stateExitCallbacks = new();
        private readonly Dictionary<ulong, List<Action>> _stateTransitionCallbacks = new();
        private readonly Dictionary<int, List<Action<float>>> _stateUpdateCallbacks = new();
        private readonly Dictionary<ulong, List<Action<float>>> _tickStateTransitionUpdateCallbacks = new();
        private readonly Dictionary<int, List<ConditionalTransition>> _stateTransitions = new();
        private readonly Dictionary<int, List<TimedStateTransitions>> _timedStateTransitions = new();

        // Local State
        private bool _isTransitioning;
        private int _localChangeCount;
        private int _stateBufferLength = MaxStateBufferLength;
        private static readonly Func<bool> tureFunc = () => true;

        private bool _performTransitionCallbacks;

        #endregion


        #region Setup

        protected virtual void Awake()
        {
            _localChangeCount = StateChangeCount - 1;
            StateMachineUpdateEnabled = true;
        }

        protected void OnDestroy()
        {
            StateMachineUpdateEnabled = false;
        }

        #endregion


        #region State Changed Callbacks

        /// <summary>
        ///     Set the default state
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        protected void SetDefaultState<T>(T state) where T : unmanaged, Enum
        {
            _defaultState = state.ConvertUnsafe<T, int>();
        }

        /// <summary>
        ///     Set the default state
        /// </summary>
        /// <param name="state"></param>
        protected void SetDefaultState(int state)
        {
            _defaultState = state;
        }

        /// <summary>
        ///     Get the active state
        /// </summary>
        /// <typeparam name="T">State enum type</typeparam>
        /// <returns>The primitive internal state converted to the passed state enum type</returns>
        public T GetCurrentState<T>()
        {
            return PrimitiveState.ConvertUnsafe<int, T>();
        }

        public bool IsState<T>(T other) where T : unmanaged, Enum
        {
            return PrimitiveState == other.ConvertUnsafe<T, int>();
        }

        public bool IsNotState<T>(T other) where T : unmanaged, Enum
        {
            return PrimitiveState != other.ConvertUnsafe<T, int>();
        }

        /// <summary>
        ///     Get the last state (if possible)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLastState<T>()
        {
            if (StateBuffer.Length >= 2)
            {
                return StateBuffer[1].State.ConvertUnsafe<int, T>();
            }
            return _defaultState.ConvertUnsafe<int, T>();
        }

        /// <summary>
        ///     Get the next state (if a state is queued)
        /// </summary>
        /// <typeparam name="T">State enum type</typeparam>
        /// <returns>The primitive internal state converted to the passed state enum type</returns>
        public T GetQueuedState<T>()
        {
            return QueuedState.ConvertUnsafe<int, T>();
        }

        /// <summary>
        ///     Called on every state transition on client and server.
        /// </summary>
        /// <param name="lastState">The state transitioning from</param>
        /// <param name="newState">The state transitioning to</param>
        protected virtual void OnStateChanged(int lastState, int newState)
        {
        }

        /// <summary>
        ///     Add a callback that is invoked every time the passed state is entered on all clients.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="callback">The logic to be executed when the state is entered</param>
        /// <typeparam name="T">State enum</typeparam>
        public void AddStateEnterCallback<T>(T state, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveState = state.ConvertUnsafe<T, int>();

            if (_stateEnterCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                _stateEnterCallbacks.Add(primitiveState, new List<Action>
                {
                    callback
                });
            }
        }

        /// <summary>
        ///     Remove a callback that is invoked every time the passed state is entered on all clients.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="callback">The logic to be executed when the state is entered</param>
        /// <typeparam name="T">State enum</typeparam>
        public void RemoveStateEnterCallback<T>(T state, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveState = state.ConvertUnsafe<T, int>();

            if (_stateEnterCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Remove(callback);
            }
        }

        /// <summary>
        ///     Add a callback that is invoked every time the passed state is exited on all clients.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="callback">The logic to be executed when the state is exited</param>
        /// <typeparam name="T">State enum</typeparam>
        public void AddStateExitCallback<T>(T state, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveState = state.ConvertUnsafe<T, int>();
            if (_stateExitCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                _stateExitCallbacks.Add(primitiveState, new List<Action>
                {
                    callback
                });
            }
        }

        /// <summary>
        ///     Remove a callback that is invoked every time the passed state is exited on all clients.
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="callback">The logic to be executed when the state is exited</param>
        /// <typeparam name="T">State enum</typeparam>
        public void RemoveStateExitCallback<T>(T state, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveState = state.ConvertUnsafe<T, int>();
            if (_stateExitCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Remove(callback);
            }
        }

        /// <summary>
        ///     Add a callback that is invoked every network tick if the state is active on all clients.
        /// </summary>
        /// <param name="state">The state during which the callback is invoked</param>
        /// <param name="callback">The logic to be executed</param>
        /// <typeparam name="T">State enum</typeparam>
        public void AddStateUpdateCallback<T>(T state, Action<float> callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveState = state.ConvertUnsafe<T, int>();
            if (_stateUpdateCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                _stateUpdateCallbacks.Add(primitiveState, new List<Action<float>>
                {
                    callback
                });
            }
        }

        /// <summary>
        ///     Remove a callback that is invoked every network tick during a specific state transition.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void RemoveStateUpdateCallback<T>(T state, Action<float> callback) where T : unmanaged, Enum
        {
            var primitiveState = state.ConvertUnsafe<T, int>();
            if (_stateUpdateCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                callbacks.Remove(callback);
            }
        }

        /// <summary>
        ///     Add a callback that is invoked every network tick during a specific state transition.
        /// </summary>
        /// <param name="from">The state from which the transition can happen from activated</param>
        /// <param name="to">The state that will be transitioned to</param>
        /// <param name="callback">The logic to be executed receiving the delta value of the transition (0 - 1)</param>
        /// <typeparam name="T">State enum</typeparam>
        public void AddStateTransitionCallback<T>(T from, T to, Action<float> callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            _performTransitionCallbacks = true;

            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();
            var combined = Combine(primitiveFrom, primitiveTo);

            if (_tickStateTransitionUpdateCallbacks.TryGetValue(combined, out var transitions))
            {
                transitions.Add(callback);
            }
            else
            {
                _tickStateTransitionUpdateCallbacks.Add(combined, new List<Action<float>>
                {
                    callback
                });
            }
        }

        /// <summary>
        ///     Remove a callback that is invoked every network tick during a specific state transition.
        /// </summary>
        public void RemoveStateTransitionCallback<T>(T from, T to, Action<float> callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            _performTransitionCallbacks = true;

            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();
            var combined = Combine(primitiveFrom, primitiveTo);

            if (_tickStateTransitionUpdateCallbacks.TryGetValue(combined, out var transitions))
            {
                transitions.Remove(callback);
            }
        }

        /// <summary>
        ///     Add a callback that is invoked once when transitioning from a specific state to another specific state.
        /// </summary>
        public void AddStateTransitionCallback<T>(T from, T to, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();
            var combined = Combine(primitiveFrom, primitiveTo);

            if (_stateTransitionCallbacks.TryGetValue(combined, out var transitions))
            {
                transitions.Add(callback);
            }
            else
            {
                _stateTransitionCallbacks.Add(combined, new List<Action>
                {
                    callback
                });
            }
        }

        /// <summary>
        ///     Remove a callback that is invoked every network tick during a specific state transition.
        /// </summary>
        public void RemoveStateTransitionCallback<T>(T from, T to, Action callback) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            _performTransitionCallbacks = true;

            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();
            var combined = Combine(primitiveFrom, primitiveTo);

            if (_stateTransitionCallbacks.TryGetValue(combined, out var transitions))
            {
                transitions.Remove(callback);
            }
        }

        /// <summary>
        ///     Determine an automatic transition between two states after an optional delay.
        /// </summary>
        /// <param name="from">The state from which the transition can happen from activated</param>
        /// <param name="to">The state that will be transitioned to</param>
        /// <param name="condition">The condition that needs to be met for the transition to happen</param>
        /// <typeparam name="T">State enum</typeparam>
        protected void AddStateTransition<T>(T from, T to, Func<bool> condition = null) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            _performTransitionCallbacks = true;

            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();

            if (_stateTransitions.TryGetValue(primitiveFrom, out var transitions))
            {
                transitions.Add(new ConditionalTransition(primitiveFrom, primitiveTo, condition ?? tureFunc));
            }
            else
            {
                _stateTransitions.Add(primitiveFrom, new List<ConditionalTransition>
                {
                    new(primitiveFrom, primitiveTo, condition ?? tureFunc)
                });
            }
        }

        /// <summary>
        ///     Determine an automatic transition between two states after an optional delay.
        /// </summary>
        /// <param name="from">The state from which the transition can happen from activated</param>
        /// <param name="to">The state that will be transitioned to</param>
        /// <param name="delayInSeconds">The delay after which the transition will happen</param>
        /// <typeparam name="T">State enum</typeparam>
        protected void AddStateTransition<T>(T from, T to, float delayInSeconds) where T : unmanaged, Enum
        {
            DebugDeclareStateType<T>();
            _performTransitionCallbacks = true;

            var primitiveFrom = from.ConvertUnsafe<T, int>();
            var primitiveTo = to.ConvertUnsafe<T, int>();

            if (_timedStateTransitions.TryGetValue(primitiveFrom, out var transitions))
            {
                transitions.Add(new TimedStateTransitions(primitiveFrom, primitiveTo, delayInSeconds));
            }
            else
            {
                _timedStateTransitions.Add(primitiveFrom, new List<TimedStateTransitions>
                {
                    new(primitiveFrom, primitiveTo, delayInSeconds)
                });
            }
        }

        /// <summary>
        ///     Cancel timed state transitions.
        /// </summary>
        protected void CancelStateTransitions()
        {
            Transition = TransitionType.None;
        }

        #endregion


        #region Blocked States

        private List<int> BlockedStates { get; } = new();

        /// <summary>
        ///     Blocks a state from being able to be entered
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        protected void BlockState<T>(T state) where T : unmanaged, Enum
        {
            var intState = state.ConvertUnsafe<T, int>();
            BlockedStates.AddUnique(intState);

            // If the current state is blocked we try to transition to the last available state.
            if (!IsState(state))
            {
                return;
            }
            var lastState = GetLastState<T>();
            Debug.Log($"Last State: {lastState}");
            var lastStateInt = lastState.ConvertUnsafe<T, int>();
            if (lastStateInt != intState)
            {
                TransitionToState(lastState);
            }
        }

        /// <summary>
        ///     Unblocks a state from being able to be entered
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        protected void UnblockState<T>(T state) where T : unmanaged, Enum
        {
            BlockedStates.Remove(state.ConvertUnsafe<T, int>());
        }

        /// <summary>
        ///     Blocks or unblocks a state based on the passed blocked value
        /// </summary>
        /// <param name="state"></param>
        /// <param name="blocked"></param>
        /// <typeparam name="T"></typeparam>
        protected void SetStateBlockedValue<T>(T state, bool blocked) where T : unmanaged, Enum
        {
            if (blocked)
            {
                BlockState(state);
            }
            else
            {
                UnblockState(state);
            }
        }

        /// <summary>
        ///     Returns true if a state is blocked and state transitions to this state are prevented
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsStateBlocked<T>(T state) where T : unmanaged, Enum
        {
            return BlockedStates.Contains(state.ConvertUnsafe<T, int>());
        }

        #endregion


        #region State Transitions

        // ReSharper disable Unity.PerformanceAnalysis
        protected void TransitionToState<T>(T newState) where T : unmanaged, Enum
        {
            TransitionToStateInternal(newState.ConvertUnsafe<T, int>());
        }

        protected void TransitionToState<T>(T newState, float secondsDelay) where T : unmanaged, Enum
        {
            TransitionToStateAfter(newState, secondsDelay);
        }

        protected void TransitionToStateAfter<T>(T newState, float secondsDelay) where T : unmanaged, Enum
        {
            QueuedState = newState.ConvertUnsafe<T, int>();
            StateTransitionTimer = new Timer(secondsDelay);
            Transition = TransitionType.Timer;
        }

        #endregion


        #region State Transtions Internal

        private void OnStateChangedInternal()
        {
            _isTransitioning = false;
            var changes = Mathf.Min(StateChangeCount - _localChangeCount, StateBufferLength);
            _localChangeCount = StateChangeCount;

            var sharedTime = Time.time;

            for (var index = changes - 1; index >= 0; index--)
            {
                try
                {
                    var fromState = StateBuffer[index + 1];
                    var toState = StateBuffer[index];
                    var difference = sharedTime - toState.TimeStamp;
                    if (index > 0 && difference > InvokeRetroactiveStateTransitionsAfter)
                    {
                        continue;
                    }
                    OnStateChangedFromToInternal(fromState.State, toState.State, difference);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }

        private void OnStateChangedFromToInternal(int fromState, int toState, float difference)
        {
            OnStateChanged(fromState, toState);

#if UNITY_EDITOR
            LogStates(fromState, toState, difference);
#endif

            // Invoke state exit callbacks
            if (_stateExitCallbacks.TryGetValue(fromState, out var exitCallbacks))
            {
                for (var i = 0; i < exitCallbacks.Count; i++)
                {
                    exitCallbacks[i].Invoke();
                }
            }

            if (_stateTransitionCallbacks.TryGetValue(Combine(fromState, toState), out var transitionCallbacks))
            {
                for (var i = 0; i < transitionCallbacks.Count; i++)
                {
                    transitionCallbacks[i].Invoke();
                }
            }

            // Invoke state enter callbacks
            if (_stateEnterCallbacks.TryGetValue(toState, out var enterCallbacks))
            {
                for (var i = 0; i < enterCallbacks.Count; i++)
                {
                    enterCallbacks[i].Invoke();
                }
            }

            // Setup automatic timed state transitions
            if (_timedStateTransitions.TryGetValue(toState, out var timedTransitions))
            {
                for (var i = 0; i < timedTransitions.Count; i++)
                {
                    timedTransitions[i].Start();
                }
            }
        }

        private void TransitionToStateInternal(int newState)
        {
            if (newState == PrimitiveState)
            {
                return;
            }
            if (BlockedStates.Contains(newState))
            {
                return;
            }

            _isTransitioning = true;

            // Enqueue
            for (var i = StateBuffer.Length - 1; i > 0; i--)
            {
                StateBuffer[i] = StateBuffer[i - 1];
            }
            StateBuffer[0] = new Buffer(newState, Time.time);

            StateTransitionTimer = Timer.None;
            Transition = TransitionType.None;
            StateChangeCount++;
            OnStateChangedInternal();
        }

        protected void UpdateStateMachine()
        {
            if (_isTransitioning)
            {
                return;
            }

            var primitiveState = PrimitiveState;

            if (_stateUpdateCallbacks.TryGetValue(primitiveState, out var callbacks))
            {
                var delta = Time.deltaTime;
                for (var i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i].Invoke(delta);
                }
            }

            if (_performTransitionCallbacks)
            {
                if (Transition == TransitionType.Timer)
                {
                    var combined = Combine(primitiveState, QueuedState);
                    if (_tickStateTransitionUpdateCallbacks.TryGetValue(combined, out var transitionCallbacks))
                    {
                        var delta = StateTransitionTimer.Delta();
                        for (var i = 0; i < transitionCallbacks.Count; i++)
                        {
                            transitionCallbacks[i].Invoke(delta);
                        }
                    }
                }

                if (_timedStateTransitions.TryGetValue(primitiveState, out var timedTransitions))
                {
                    foreach (var timedTransition in timedTransitions)
                    {
                        var combined = Combine(PrimitiveState, timedTransition.ToState);

                        if (_tickStateTransitionUpdateCallbacks.TryGetValue(combined, out var transitionCallbacks))
                        {
                            var delta = timedTransition.Timer.Delta();
                            for (var j = 0; j < transitionCallbacks.Count; j++)
                            {
                                transitionCallbacks[j].Invoke(delta);
                            }
                        }
                        if (timedTransition.Timer.Expired)
                        {
                            TransitionToStateInternal(timedTransition.ToState);
                            return;
                        }
                    }
                }
            }

            if (_stateTransitions.TryGetValue(primitiveState, out var transitions))
            {
                for (var i = 0; i < transitions.Count; i++)
                {
                    if (transitions[i].Condition())
                    {
                        TransitionToStateInternal(transitions[i].ToState);
                        return;
                    }
                }
            }

            if (Transition == TransitionType.Timer && StateTransitionTimer.Expired)
            {
                TransitionToStateInternal(QueuedState);
            }
        }

        #endregion


        #region Helper

        private static ulong Combine(int a, int b)
        {
            var ua = (uint) a;
            ulong ub = (uint) b;
            return ub << 32 | ua;
        }

        private static void Separate(ulong c, out int a, out int b)
        {
            a = (int) (c & 0xFFFFFFFFUL);
            b = (int) (c >> 32);
        }

        #endregion


        #region Debug

        /// <summary>
        ///     Declare the type used by the state machine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Conditional("UNITY_EDITOR")]
        protected void DebugDeclareStateType<T>() where T : unmanaged, Enum
        {
#if UNITY_EDITOR
            _debugType = typeof(T);
#endif
        }

        /// <summary>
        ///     Declare the type used by the state machine
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected void DebugSetColor(Color color)
        {
#if UNITY_EDITOR
            _debugColor = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
#endif
        }

#if UNITY_EDITOR

        private const string FromColor = "<color=#7693cc>";
        private const string ToColor = "<color=#9e76cc>";
        private const string EndColor = "</color>";
        private Type _debugType;
        private string _debugColor;
        private static int colorIndex;
        private static readonly Color[] colors =
        {
            new(1f, 0.36f, 0.4f), new(1f, 0.51f, 0.33f),
            new(0.64f, 0.63f, 1f), new(0.75f, 1f, 0.62f),
            new(0.75f, 0.63f, 1f), new(1f, 1f, 0.55f),
            new(0.6f, 0.45f, 1f), new(1f, 0.74f, 0.25f)
        };
        private string DebugColor => string.IsNullOrWhiteSpace(_debugColor) ? _debugColor = $"<color=#{ColorUtility.ToHtmlStringRGB(colors[Next()])}>" : _debugColor;

        private static int Next()
        {
            colorIndex++;
            if (colorIndex > colors.Length - 1)
            {
                colorIndex = 0;
            }
            return colorIndex;
        }

        private static readonly LogCategory category = "State Machine";

        private void LogStates(int fromState, int toState, float difference)
        {
            Debug.Log(category, _debugType != null
                ? $"{DebugColor}[{GetType().Name}]{EndColor} Transitioned from {FromColor}[{Enum.ToObject(_debugType, fromState)}({fromState.ToString()})]{EndColor} to {ToColor}[{Enum.ToObject(_debugType, toState)}({toState.ToString()})]{EndColor} after [{difference.ToString("0.000s")}]"
                : $"{DebugColor}[{GetType().Name}]{EndColor} Transitioned from {FromColor}[{fromState.ToString()}]{EndColor} to {ToColor}[{toState.ToString()}]{EndColor} after [{difference.ToString("0.000s")}]");
        }
#endif

        #endregion
    }
}
