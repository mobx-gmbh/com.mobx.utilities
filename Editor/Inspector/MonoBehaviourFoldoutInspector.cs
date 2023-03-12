using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
    [UnityEditor.CanEditMultipleObjects]
#endif
    public class MonoBehaviourFoldoutInspector : OverrideInspector<MonoBehaviour>
    {
    }
}
