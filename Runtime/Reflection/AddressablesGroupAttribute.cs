using System;

namespace MobX.Utilities.Reflection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddressablesGroupAttribute : Attribute
    {
        public string GroupName { get; }

        public AddressablesGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}