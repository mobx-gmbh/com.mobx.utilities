using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class ReadonlyInspectorAttribute : PropertyAttribute
    {
    }
}