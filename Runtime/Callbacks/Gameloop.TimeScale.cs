using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public partial class Gameloop
    {
        private static readonly List<Func<float>> timeScaleModifier = new();

        private static float CalculateTimeScale()
        {
            var modifier = 1f;
            foreach (var func in timeScaleModifier)
            {
                modifier *= func().WithMinLimit(0);
            }

            return Time.timeScale * modifier;
        }
    }
}