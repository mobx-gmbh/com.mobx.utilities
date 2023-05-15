using MobX.Utilities.Collections;
using UnityEngine;

namespace MobX.Utilities.Editor.AssetIcons
{
    public class AssetIconMappings : ScriptableObject
    {
        [SerializeField] private Map<UnityEditor.MonoScript, Texture2D> scriptIcons = new();
        public Map<UnityEditor.MonoScript, Texture2D> ScriptIcons => scriptIcons;
    }
}
