using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Unity
{
    public static class PhysicsUtilities
    {
        public static bool CheckSpheres(IEnumerable<Vector3> positions, float radius, LayerMask layerMask,
            QueryTriggerInteraction interaction = QueryTriggerInteraction.Ignore)
        {
            foreach (var position in positions)
            {
                if (Physics.CheckSphere(position, radius, layerMask, interaction))
                {
                    return true;
                }
            }
            return false;
        }
    }
}