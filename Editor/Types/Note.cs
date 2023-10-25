using JetBrains.Annotations;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Types
{
    [HideMonoScript]
    public class Note : ScriptableObject
    {
#pragma warning disable
        [TextArea(0, 30)]
        [UsedImplicitly]
        [Label("Developer Note")]
        [SerializeField] private string note;
#pragma warning restore
    }
}