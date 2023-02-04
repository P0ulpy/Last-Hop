using System;
using Core;
using UnityEngine;

namespace Props.Enemies.Supercopter
{
    public class Supercopter : BaseEnemy
    {
        [SerializeField] private GameObject projectilePrefab;

        [Header("Config")]
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float closestDistanceFromTarget = 3.5f;
        [SerializeField] private float shootCooldown = 5.0f;
        [SerializeField] private GameObject explosionVFX;

        private float _timeSinceLastShoot = 0f;
        private bool _haveProjectile = false;

        private void Update()
        {
            if(_targetTransform == null)
                return;
            
            if (_timeSinceLastShoot <= shootCooldown)
            {
                _timeSinceLastShoot += Time.deltaTime;
                
                Move();
            }
            else if(!_haveProjectile)
                Shoot();
        }

        private void Shoot()
        {
            _haveProjectile = true;
            
            var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<SupercopterProjectile>();
            projectile.Build(transform, _targetTransform, () =>
            {
                _haveProjectile = false;
                _timeSinceLastShoot = 0f;
                Destroy(projectile.gameObject);
            });
        }

        private void Move()
        {
            if (IsAtClosestDistance())
                return;
                
            var xDirection = Utils.GetXDirection(transform.position, _targetTransform.position);
            
            UpdatePosition(xDirection);
            UpdateRotation(xDirection);
        }
        
        private bool IsAtClosestDistance()
        {
            return Math.Abs(transform.position.x - _targetTransform.position.x) < closestDistanceFromTarget;
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

        private void OnDisable()
        {
            var vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            vfx.transform.localScale = transform.localScale / 2;
        }
    }
}