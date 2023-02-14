﻿using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobX.Utilities.Types
{
    [StructLayout(LayoutKind.Auto)]
    public struct Loop
    {
        #region Properties

        public int Iterations => iterations;

        #endregion


        #region Fields

        private int value;
        private int iterations;
        private readonly int startValue;
        private readonly int max;
        private readonly int min;

        #endregion


        #region Factory

        public static Loop Create(int min, int max)
        {
            return new Loop(0, min, max);
        }

        public static Loop Create(int max)
        {
            return new Loop(0, 0, max);
        }

        public static Loop Create<T>(IList<T> list)
        {
            return new Loop(0, 0, list.Count - 1);
        }

        public static Loop Create(int startIndex, int min, int max)
        {
            return new Loop(startIndex, min, max);
        }

        public static Loop Create<T>(int startIndex, IList<T> list)
        {
            return new Loop(startIndex, 0, list.Count - 1);
        }

        public Loop(int value, int min, int max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
            startValue = value;
            iterations = 0;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        #endregion


        #region Operator

        public static Loop operator ++(Loop looping)
        {
            if (++looping.value > looping.max)
            {
                looping.value = looping.min;
            }

            if (looping.value == looping.startValue)
            {
                looping.iterations++;
            }

            return looping;
        }

        public static Loop operator --(Loop looping)
        {
            if (--looping.value < looping.min)
            {
                looping.value = looping.max;
            }

            if (looping.value == looping.startValue)
            {
                looping.iterations--;
            }

            return looping;
        }

        public static implicit operator int(Loop loop)
        {
            return loop.value;
        }

        #endregion
    }
}
