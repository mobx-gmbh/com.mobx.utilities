using System;
using UnityEngine;

namespace MobX.Utilities.DOTS
{
    [Serializable]
    public class Curve
    {
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private int resolution = 32;

        public NativeCurve NativeCurve => new(animationCurve, resolution);
        public AnimationCurve AnimationCurve => animationCurve;
        public int Resolution => resolution;

        public static implicit operator NativeCurve(Curve settings)
        {
            return settings.NativeCurve;
        }
    }
}