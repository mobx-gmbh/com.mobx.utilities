using MobX.Utilities.Callbacks;
using MobX.Utilities.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Cooldown
{
    public class CooldownManager : SingletonAsset<CooldownManager>
    {
        #region Public

        /// <summary>
        ///     Start a new cooldown.
        /// </summary>
        public static void StartCooldown(ICooldownCallback target, float cooldownDuration)
        {
            Singleton.StartCooldownInternal(target, cooldownDuration);
        }

        /// <summary>
        ///     Reduce the cooldown by a given time in seconds.
        /// </summary>
        public static void ReduceCooldown(ICooldownCallback target, float reductionInSeconds)
        {
            Singleton.ReduceCooldownInternal(target, reductionInSeconds);
        }

        /// <summary>
        ///     Update the duration of a running cooldown.
        /// </summary>
        public static void SetRemainingCooldown(ICooldownCallback target, float newCooldownDuration)
        {
            Singleton.UpdateCooldownInternal(target, newCooldownDuration);
        }

        /// <summary>
        ///     Instantly cancel the cooldown without raising completion callbacks.
        /// </summary>
        /// <param name="target"></param>
        public static void CancelCooldown(ICooldownCallback target)
        {
            Singleton.CancelCooldownInternal(target);
        }

        /// <summary>
        ///     Instantly complete the cooldown and raise completion callbacks.
        /// </summary>
        public static float GetRemainingCooldown(ICooldownCallback target)
        {
            return Singleton.GetRemainingCooldownInternal(target);
        }

        /// <summary>
        ///     Instantly complete the cooldown and raise completion callbacks.
        /// </summary>
        public static void CompleteCooldown(ICooldownCallback target)
        {
            Singleton.CompleteCooldownInternal(target);
        }

        /// <summary>
        ///     Cancel all active cooldowns.
        /// </summary>
        public static void CancelAllCooldowns()
        {
            Singleton.CancelAllCooldownsInternal();
        }

        public static bool IsOnCooldown(ICooldownCallback target)
        {
            return Singleton.IsOnCooldownInternal(target);
        }

        public static bool IsNotOnCooldown(ICooldownCallback target)
        {
            return Singleton.IsOnCooldownInternal(target) is false;
        }

        #endregion


        #region Internal

        private readonly List<(ICooldownCallback callback, float endTimestamp)> _cooldowns = new();

        private void StartCooldownInternal(ICooldownCallback callbackReceiver, float cooldownDuration)
        {
            callbackReceiver.OnBeginCooldown();

            var newEntry = new ValueTuple<ICooldownCallback, float>(callbackReceiver, Time.time + cooldownDuration);
            for (var index = _cooldowns.Count - 1; index >= 0; index--)
            {
                var entry = _cooldowns[index];
                if (entry.callback == callbackReceiver)
                {
                    _cooldowns[index] = newEntry;
                    return;
                }
            }
            _cooldowns.Add(newEntry);
        }

        private void UpdateCooldownInternal(ICooldownCallback callbackReceiver, float cooldownDuration, bool callOnBegin = false)
        {
            for (var index = _cooldowns.Count - 1; index >= 0; index--)
            {
                var entry = _cooldowns[index];
                if (entry.callback != callbackReceiver)
                {
                    continue;
                }

                _cooldowns[index] = new ValueTuple<ICooldownCallback, float>(callbackReceiver, Time.time + cooldownDuration);
                if (callOnBegin)
                {
                    callbackReceiver.OnBeginCooldown();
                }
                return;
            }
        }

        private void CancelCooldownInternal(ICooldownCallback callbackReceiver)
        {
            for (var index = _cooldowns.Count - 1; index >= 0; index--)
            {
                var entry = _cooldowns[index];
                if (entry.callback != callbackReceiver)
                {
                    continue;
                }

                _cooldowns.RemoveAt(index);
                return;
            }
        }

        private float GetRemainingCooldownInternal(ICooldownCallback target)
        {
            foreach (var entry in _cooldowns)
            {
                if (entry.callback == target)
                {
                    return (entry.endTimestamp - Time.time).WithMinLimit(0);
                }
            }
            return 0;
        }

        private void CompleteCooldownInternal(ICooldownCallback callbackReceiver)
        {
            for (var index = _cooldowns.Count - 1; index >= 0; index--)
            {
                var entry = _cooldowns[index];
                if (entry.callback != callbackReceiver)
                {
                    continue;
                }

                _cooldowns.RemoveAt(index);
                entry.callback.OnEndCooldown();
                return;
            }
        }

        private void ReduceCooldownInternal(ICooldownCallback callbackReceiver, float reductionInSeconds)
        {
            for (var index = 0; index < _cooldowns.Count; index++)
            {
                if (_cooldowns[index].callback == callbackReceiver)
                {
                    _cooldowns[index] = (callbackReceiver, _cooldowns[index].endTimestamp - reductionInSeconds);
                }
            }
        }

        private void CancelAllCooldownsInternal()
        {
            foreach (var entry in _cooldowns)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (entry.callback is IOnCooldownCancelled cancelledCallback)
                {
                    cancelledCallback.OnCooldownCancelled();
                }
            }
            _cooldowns.Clear();
        }

        private bool IsOnCooldownInternal(ICooldownCallback target)
        {
            foreach (var entry in _cooldowns)
            {
                if (entry.callback == target)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Gameloop

        [CallbackOnUpdate]
        private void OnUpdate()
        {
            var timeStamp = Time.time;

            for (var index = _cooldowns.Count - 1; index >= 0; index--)
            {
                var entry = _cooldowns[index];
                entry.callback.OnCooldownUpdate(entry.endTimestamp - timeStamp);

                if (timeStamp > entry.endTimestamp)
                {
                    _cooldowns.RemoveAt(index);
                    entry.callback.OnEndCooldown();
                }
            }
        }

        #endregion
    }
}
