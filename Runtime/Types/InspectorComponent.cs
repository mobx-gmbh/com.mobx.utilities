using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities
{
    public class InspectorComponent : MonoBehaviour
    {
        [InlineInspector]
        [SerializeField] private Component inlined;
    }
}
