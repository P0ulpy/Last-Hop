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
            if(!IsAtClosestDistance())
                UpdatePosition();
        }

        private bool IsAtClosestDistance()
        {
            return Math.Abs(transform.position.x - targetTransform.position.x) < closestDistanceFromTarget;
        }
        
        
        
        private void UpdatePosition()
        {
            var position = transform.position;
            var xDirection = Utils.GetXDirection(position, targetTransform.position);
            var newX = position.x + (xDirection * (speed * Time.deltaTime));
            
            transform.position = new Vector2(newX, position.y);
        }
    }
}