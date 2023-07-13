using System;
using UnityEngine;

namespace MobX.Utilities.Types
{
    [Serializable]
    public struct RuntimeGuid
    {
        public string Value => value;

        [SerializeField] private string value;

        public RuntimeGuid(string value)
        {
            this.value = value;
        }

        public static implicit operator RuntimeGuid(string value)
        {
            return new RuntimeGuid(value);
        }

        public static implicit operator string(RuntimeGuid guid)
        {
            return guid.value;
        }

#if UNITY_EDITOR

        public static implicit operator RuntimeGuid(UnityEditor.GUID value)
        {
            return new RuntimeGuid(value.ToString());
        }

#endif

        public override string ToString()
        {
            return value;
        }
    }
}
