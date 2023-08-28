using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true)]
    [UnityEditor.CanEditMultipleObjects]
#endif
    public class ScriptableObjectFoldoutInspector : OverrideInspector<ScriptableObject>
    {
    }
}
