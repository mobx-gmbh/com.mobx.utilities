using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Registry
{
    public static class RuntimeGUIDExtensions
    {
        public static T ToAsset<T>(this RuntimeGUID guid) where T : Object
        {
            return AssetRegistry.Resolve<T>(guid);
        }
    }
}
