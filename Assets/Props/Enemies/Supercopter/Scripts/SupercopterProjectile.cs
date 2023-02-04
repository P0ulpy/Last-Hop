using UnityEngine;

namespace Props.Enemies.Supercopter
{
    public class SupercopterProjectile : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private float curveOffset = 5f;
        [SerializeField] private AnimationCurve curve;

        private bool _isInShoot = false;
        
        private Vector3 _shootOrigin;
        private Transform _targetTransform;
        
        private float _t = 0f;

        public void Build(Transform shooterTransform, Transform targetTransform)
        {
            _shootOrigin = shooterTransform.position;
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
            _t += speed * Time.deltaTime;
            
            var targetPosition = _targetTransform.position;

            float factor = curve.Evaluate(_t);

            var target = new Vector2(
                targetPosition.x,
                targetPosition.y + (factor * curveOffset)
            );

            transform.position = Vector3.Lerp(_shootOrigin, target, _t);
        }
    }
}