using System;
using UnityEngine;

namespace MobX.Utilities.Types
{
    [Serializable]
    public struct RuntimeGUID
    {
        public string Value => value;

        [SerializeField] private string value;

        public RuntimeGUID(string value)
        {
            this.value = value;
        }

        public static implicit operator RuntimeGUID(string value)
        {
            return new RuntimeGUID(value);
        }

        public static implicit operator string(RuntimeGUID guid)
        {
            return guid.value;
        }

#if UNITY_EDITOR

        public static implicit operator RuntimeGUID(UnityEditor.GUID value)
        {
            return new RuntimeGUID(value.ToString());
        }

#endif

        public override string ToString()
        {
            return value;
        }
    }
}
