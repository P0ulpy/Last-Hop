using System;
using Core;
using UnityEngine;

namespace Props.Enemies.Supercopter
{
    public class Supercopter : BaseEnemy
    {
        [SerializeField] private Transform targetTransform;
        
        [Header("Config")]
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float closestDistanceFromTarget = 3.5f;
        [SerializeField] private float shootCooldown = 5.0f;

        [Header("Shoot config")] 
        [SerializeField] private GameObject projectilePrefab;

        private float _timeSinceLastShoot = 0f;

        private void Update()
        {
            /*if (_timeSinceLastShoot <= shootCooldown)
            {
                _timeSinceLastShoot += Time.deltaTime;
                
                Move();
            }
            else
                Shoot();*/
            
            Move();
            Shoot();
        }

        private void Shoot()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<SupercopterProjectile>();
                projectile.Build(transform, targetTransform);
            }
        }

        private void Move()
        {
            if (IsAtClosestDistance())
                return;
                
            var xDirection = Utils.GetXDirection(transform.position, targetTransform.position);
            
            UpdatePosition(xDirection);
            UpdateRotation(xDirection);
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