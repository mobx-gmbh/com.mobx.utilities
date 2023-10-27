using JetBrains.Annotations;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Types
{
    [HideMonoScript]
    public class Todo : ScriptableObject
    {
#pragma warning disable
        [TextArea(0, 30)]
        [UsedImplicitly]
        [SerializeField] private string todo;
#pragma warning restore
    }
}