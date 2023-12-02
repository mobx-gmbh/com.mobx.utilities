using JetBrains.Annotations;
using UnityEngine;

namespace MobX.Utilities.Editor.Types
{
    public class Todo : ScriptableObject
    {
#pragma warning disable
        [TextArea(0, 30)]
        [UsedImplicitly]
        [SerializeField] private string todo;
#pragma warning restore
    }
}