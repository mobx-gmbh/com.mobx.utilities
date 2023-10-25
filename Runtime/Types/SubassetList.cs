using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Types
{
    [Serializable]
    public struct SubassetList<T> where T : ScriptableObject
    {
        [SerializeField] private T[] assets;

        public IReadOnlyCollection<T> Assets => assets;
    }
}