using Fusion;

namespace MobX.Utilities.Fusion
{
    public readonly struct SharedTimer : INetworkStruct
    {
        private readonly float _targetTime;
        private readonly float _startTime;

        public SharedTimer(float delayInSeconds)
        {
            if (Session.IsInitialized is false)
            {
                Debug.LogError("Session", "Cannot create a timer before the session started!");
                _targetTime = 0;
                _startTime = 0;
            }
            else
            {
                _startTime = Session.SharedTime;
                _targetTime = _startTime + delayInSeconds;
            }
        }

        public bool IsRunning => _targetTime > Session.SharedTime;

        public bool Expired => 0 < _targetTime && _targetTime <= Session.SharedTime;

        public bool ExpiredOrNotRunning => _targetTime <= Session.SharedTime;

        public float? RemainingTime => IsRunning ? _targetTime - Session.SharedTime : null;

        public float? SmoothRemainingTime => IsRunning ? _targetTime - Session.SmoothSharedTime : null;

        public float Delta(float fallback = 0)
        {
            if (IsRunning)
            {
                var totalDuration = _targetTime - _startTime;
                var passedTime = totalDuration - SmoothRemainingTime!.Value;
                return passedTime / totalDuration;
            }
            return fallback;
        }

        public override string ToString()
        {
            return SmoothRemainingTime.ToString();
        }

        public static SharedTimer None => new();

        public bool TryGetRemainingTime(out float remainingTime)
        {
            if (Expired)
            {
                remainingTime = 0;
                return true;
            }
            if (IsRunning)
            {
                remainingTime = RemainingTime!.Value;
                return true;
            }

            remainingTime = 0;
            return false;
        }
    }
}
