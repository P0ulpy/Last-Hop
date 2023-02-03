using UnityEngine;

namespace Core
{
    public class Utils
    {
        public static float GetXDirection(Transform a, Transform b) { return GetXDirection(a.position, b.position); }
        public static float GetXDirection(Vector2 a, Vector2 b)
        {
            var direction = (b - a).normalized;
            return direction.x > 0 ? 1 : -1;
        }
    }
}