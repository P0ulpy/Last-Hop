using System;
using Core;
using UnityEngine;

namespace Props.Supercopter.Scripts
{
    public class Supercopter : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        
        [Header("Config")]
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float closestDistanceFromTarget = 3.5f;
        [SerializeField] private float shootCooldown = 5.0f;

        private void Update()
        {
            if (!IsAtClosestDistance())
            {
                var xDirection = Utils.GetXDirection(transform.position, targetTransform.position);
                
                UpdatePosition(xDirection);
                UpdateRotation(xDirection);
            }
        }

        private bool IsAtClosestDistance()
        {
            return Math.Abs(transform.position.x - targetTransform.position.x) < closestDistanceFromTarget;
        }

        private void UpdatePosition(float xDirection)
        {
            var position = transform.position;
            var newX = position.x + (xDirection * (speed * Time.deltaTime));
            
            transform.position = new Vector2(newX, position.y);
        }

        private void UpdateRotation(float xDirection)
        {
            transform.rotation = Utils.GetYRotationFromXDirection(xDirection, transform.rotation);
        }
    }
}