using UnityEngine;

namespace MobX.Utilities.Editor
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true)]
#endif
    public class ScriptableObjectFoldoutInspector : OverrideInspector<ScriptableObject>
    {
    }
}