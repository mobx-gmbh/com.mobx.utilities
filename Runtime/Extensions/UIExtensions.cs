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

        public static Rect GetScreenCoordinatesOfCorners(this RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            var result = new Rect(
                worldCorners[0].x,
                worldCorners[0].y,
                worldCorners[2].x - worldCorners[0].x,
                worldCorners[2].y - worldCorners[0].y);
            return result;
        }

        public static Vector2 GetPixelPositionOfRect(this RectTransform uiElement)
        {
            var screenRect = GetScreenCoordinatesOfCorners(uiElement);

            return new Vector2(screenRect.center.x, screenRect.center.y);
        }

        #endregion
    }
}