using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject
    {
        protected RuntimeAsset()
        {
            EngineCallbacks.AddCallbacks(this);
        }

        [Conditional("UNITY_EDITOR")]
        protected static void AssertIsPlaying()
        {
            Assert.IsTrue(Application.isPlaying, "Application Is Not Playing!");
        }
    }
}