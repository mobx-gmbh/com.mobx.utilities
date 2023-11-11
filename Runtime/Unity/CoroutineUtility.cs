using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Unity
{
    public class CoroutineUtility
    {
        private static readonly Dictionary<float, WaitForSeconds> waitForSecondsCache = new();

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (!waitForSecondsCache.TryGetValue(seconds, out var waitForSeconds))
            {
                waitForSeconds = new WaitForSeconds(seconds);
                waitForSecondsCache.Add(seconds, waitForSeconds);
            }

            return waitForSeconds;
        }
    }
}
