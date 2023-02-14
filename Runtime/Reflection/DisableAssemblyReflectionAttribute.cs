// Copyright (c) 2022 Jonathan Lang

using MobX.Utilities.Reflection;
using System;

[assembly: DisableAssemblyReflection]
namespace MobX.Utilities.Reflection
{
    /// <summary>
    /// Disable reflection for the target assembly or class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct)]
    public class DisableAssemblyReflectionAttribute : Attribute
    {
    }
}
