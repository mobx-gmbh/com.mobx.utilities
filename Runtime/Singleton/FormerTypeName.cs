using System;

namespace MobX.Utilities.Singleton
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FormerTypeName : Attribute
    {
        public readonly string TypeName;

        public FormerTypeName(string formerTypeName)
        {
            TypeName = formerTypeName;
        }
    }
}
