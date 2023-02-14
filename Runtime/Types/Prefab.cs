using JetBrains.Annotations;
using System;
using UnityEngine;

namespace MobX.Utilities.Types
{
    [Serializable]
    public class Prefab
    {
        [UsedImplicitly]
        public bool enabled = true;
        public GameObject gameObject;

        public static implicit operator GameObject(Prefab prefab)
        {
            return prefab.gameObject;
        }
    }
}
