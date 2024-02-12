﻿using UnityEngine;

namespace MobX.Utilities.Types
{
    public readonly struct UnscaledTimer
    {
        // State
        private readonly float _targetTime;
        private readonly float _startTime;

        public UnscaledTimer(float delayInSeconds)
        {
            _startTime = Time.unscaledTime;
            _targetTime = _startTime + delayInSeconds;
        }

        public bool IsRunning => _targetTime > Time.unscaledTime;

        public bool Expired => 0 < _targetTime && _targetTime <= Time.unscaledTime;

        public bool ExpiredOrNotRunning => _targetTime <= Time.unscaledTime;

        public float RemainingTime => IsRunning ? _targetTime - Time.unscaledTime : 0;

        public float Delta(float fallback = 0)
        {
            if (IsRunning)
            {
                var totalDuration = _targetTime - _startTime;
                var passedTime = totalDuration - RemainingTime;
                return passedTime / totalDuration;
            }
            return fallback;
        }

        public override string ToString()
        {
            return $"{nameof(UnscaledTimer)}: {_targetTime}";
        }

        public static UnscaledTimer None => new();

        public static UnscaledTimer FromSeconds(float durationInSeconds)
        {
            return new UnscaledTimer(durationInSeconds);
        }
    }
}