using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class TexturePreviewAttribute : PropertyAttribute
    {
        public readonly int PreviewFieldHeight;

        public TexturePreviewAttribute(int previewFieldHeight = 80)
        {
            PreviewFieldHeight = previewFieldHeight;
        }
    }
}