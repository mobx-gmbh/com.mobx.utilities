using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Utilities
{
    public static class UIExtensions
    {
        #region Rect Transform

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRectInsideScreen(this RectTransform rectTransform, float tolerance)
        {
            var inside = true;
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var displayRect = new Rect(-tolerance, -tolerance, Screen.width + tolerance * 2,
                Screen.height + tolerance * 2);

            foreach (var corner in corners)
            {
                if (!displayRect.Contains(corner))
                {
                    inside = false;
                }
            }

            return inside;
        }

        #endregion
    }
}
