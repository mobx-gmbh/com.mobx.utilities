using UnityEngine;

namespace MobX.Utilities.Tools
{
    public class DrawUtility
    {
        public static void DrawCapsuleCast(Vector3 start, Vector3 end, float radius, Vector3 direction, float distance,
            bool hit)
        {
            var color = hit ? Color.green : Color.red;

            // Draw top and bottom circles
            DrawCircle(start, radius, color);
            DrawCircle(end, radius, color);

            // Draw lines to represent the sides of the capsule
            Debug.DrawRay(start, direction * distance, color);
            Debug.DrawRay(end, direction * distance, color);

            // Connect the circles
            var directionNormal = direction.normalized;
            const float AngleStep = 360.0f / 12; // Adjust number of lines for sides
            for (var i = 0; i < 12; i++)
            {
                var circlePoint = Quaternion.Euler(0, AngleStep * i, 0) * Vector3.forward * radius;
                Debug.DrawLine(start + circlePoint, end + circlePoint, color);
            }

            // Draw the hemispheres
            DrawHemisphere(start, radius, color, true); // Top hemisphere
            DrawHemisphere(end, radius, color, false); // Bottom hemisphere
        }

        private static void DrawHemisphere(Vector3 center, float radius, Color color, bool top)
        {
            const float AngleStep = 360.0f / 20;
            const float HemisphereStep = 90.0f / 10; // Adjust for hemisphere smoothness

            for (var angle = 0; angle <= 360; angle += (int) AngleStep)
            {
                var previousPoint = Vector3.zero;
                for (var hemisphereAngle = 0; hemisphereAngle <= 90; hemisphereAngle += (int) HemisphereStep)
                {
                    var rotation = Quaternion.Euler(hemisphereAngle * (top ? 1 : -1), angle, 0);
                    var newPoint = center + rotation * Vector3.forward * radius;

                    if (previousPoint != Vector3.zero)
                    {
                        Debug.DrawLine(previousPoint, newPoint, color);
                    }

                    previousPoint = newPoint;
                }
            }
        }

        public static void DrawCircle(Vector3 center, float radius, Color color)
        {
            const float AngleStep = 360.0f / 20; // Adjust for circle smoothness
            var previousPoint = center + Quaternion.Euler(0, 0, 0) * Vector3.forward * radius;

            for (var i = 1; i <= 20; i++)
            {
                var newPoint = center + Quaternion.Euler(0, AngleStep * i, 0) * Vector3.forward * radius;
                Debug.DrawLine(previousPoint, newPoint, color);
                previousPoint = newPoint;
            }
        }
    }
}