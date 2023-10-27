using MobX.Utilities.Types;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(InlinedScriptableObject), true)]
    public class InlinedAssetPropertyDrawer : InlineInspectorDrawer
    {
    }
}