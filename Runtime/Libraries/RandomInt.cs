using System;
using System.Collections.Generic;

namespace MobX.Utilities.Libraries
{
    public readonly struct RandomInt
    {
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly int _distinctCount;
        private readonly Queue<int> _lastValues;
        private static readonly Random random = new();

        public RandomInt(int minValue, int maxValue, int distinctCount)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("minValue should be less than maxValue");
            }
            if (distinctCount > maxValue - minValue)
            {
                throw new ArgumentException("distinctCount should be less than the range of possible values");
            }

            _minValue = minValue;
            _maxValue = maxValue;
            _distinctCount = distinctCount;
            _lastValues = new Queue<int>(distinctCount);
        }

        public int Sample()
        {
            int newValue;

            do
            {
                newValue = random.Next(_minValue, _maxValue);
            }
            while (_lastValues.Contains(newValue));

            _lastValues.Enqueue(newValue);

            if (_lastValues.Count > _distinctCount)
            {
                _lastValues.Dequeue();
            }

            return newValue;
        }
    }
}