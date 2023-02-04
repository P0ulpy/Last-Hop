using UnityEngine;

namespace Props.Supercopter.Scripts
{
    public class SupercopterProjectile : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private float speed = 0.8f;
        [SerializeField] private float curveOffset = 5f;
        [SerializeField] private AnimationCurve curve;

        private bool _isOnShoot = false;
        
        private float _shootDuration = 0f;
        private Transform _shooterTransform;
        private Transform _targetTransform;

        private float _shootTimer = 0f;
        
        public void Build(Transform shooterTransform, Transform targetTransform)
        {
            _shooterTransform = shooterTransform;
            _targetTransform = targetTransform;

            _shootDuration = 5f;
            //_shootDuration = Vector3.Distance(_targetTransform.position, _shooterTransform.position) * shootDurationFactor;
            
            _isOnShoot = true;
        }
        
        private void Update()
        {
            if (!_isOnShoot)
                return;
            
            _shootTimer += Time.deltaTime;
            
            Shoot();
        }

        private void Shoot()
        {
            float t = (_shootTimer / _shootDuration);

            var shooterPosition = _shooterTransform.position;
            var targetPosition = _targetTransform.position;

            float factor = curve.Evaluate(t);

            var target = new Vector2(
                targetPosition.x,
                targetPosition.y + (factor * curveOffset)
            );

            transform.position = Vector3.Lerp(shooterPosition, target, t);
        }
    }
}