using UnityEngine;

namespace MobX.Utilities.Editor
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
#endif
    public class MonoBehaviourFoldoutInspector : OverrideInspector<MonoBehaviour>
    {
    }
}