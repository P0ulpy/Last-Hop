﻿using UnityEngine;

namespace Core
{
    public static class Utils
    {
        public static float GetXDirection(Transform a, Transform b) { return GetXDirection(a.position, b.position); }
        public static float GetXDirection(Vector2 a, Vector2 b)
        {
            var direction = (b - a).normalized;
            return direction.x > 0 ? 1 : -1;
        }

        public static Quaternion GetYRotationFromXDirection(float xDirection, Quaternion rotation) { return GetYRotationFromXDirection(xDirection, rotation.eulerAngles); }

        public static Quaternion GetYRotationFromXDirection(float xDirection, Vector3 rotation)
        {
            return Quaternion.Euler(xDirection > 0 
                ? new Vector3(rotation.x, 0, rotation.z) 
                : new Vector3(rotation.x, -180, rotation.z)
            );
        }
        
        public static Quaternion GetZRotationFromPositions(Vector3 positionA, Vector3 positionB)
        {
            var direction = (positionB - positionA).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}