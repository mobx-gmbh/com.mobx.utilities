using System;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class EndBoxAttribute : PropertyAttribute
    {
    }
}