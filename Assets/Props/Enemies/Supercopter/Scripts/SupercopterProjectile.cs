using UnityEngine;

namespace Props.Enemies.Supercopter
{
    public class SupercopterProjectile : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private float speed = 0.8f;
        [SerializeField] private float curveOffset = 5f;
        [SerializeField] private AnimationCurve curve;

        private bool _isInShoot = false;
        
        private Transform _shooterTransform;
        private Transform _targetTransform;

        public void Build(Transform shooterTransform, Transform targetTransform)
        {
            _shooterTransform = shooterTransform;
            _targetTransform = targetTransform;

            _isInShoot = true;
        }
        
        private void Update()
        {
            if (!_isInShoot)
                return;
            
            Shoot();
        }

        private void Shoot()
        {
            float t = 0.5f;

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