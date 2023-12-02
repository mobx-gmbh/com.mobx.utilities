using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Types
{
    public class Note : ScriptableObject
    {
#pragma warning disable
        [TextArea(0, 30)]
        [UsedImplicitly]
        [LabelText("Developer Note")]
        [SerializeField] private string note;
#pragma warning restore
    }
}