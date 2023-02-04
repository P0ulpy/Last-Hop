using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Props.Enemies.Supercopter
{
    public class SupercopterProjectile : MonoBehaviour
    {
        [SerializeField] private GameObject explosionFx;

        [Header("Config")] 
        [SerializeField] private int damages = 20;
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private float curveOffset = 5f;
        [SerializeField] private AnimationCurve curve;

        private bool _isInShoot = false;
        
        private Vector3 _shootOrigin;
        private Vector3 _target;
        private UnityAction _onHitCallBack;
        
        private float _t = 0f;
        private int _factorDirection = 1;

        public void Build(Transform shooterTransform, Transform targetTransform, UnityAction onHit = null)
        {
            _shootOrigin = shooterTransform.position;
            _target = targetTransform.position;
            _onHitCallBack = onHit;
            
            _isInShoot = true;
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.O))
                Deflect();
#endif
            
            if (!_isInShoot)
                return;
            
            Shoot();
        }

        public void Deflect()
        {
            _target = _shootOrigin;
            _factorDirection = -1;
        }

        private void Shoot()
        {
            _t += speed * _factorDirection * Time.deltaTime;
            
            var targetPosition = _target;

            float factor = curve.Evaluate(_t);

            var target = new Vector2(
                targetPosition.x,
                targetPosition.y + (factor * curveOffset)
            );

            transform.position = Vector3.Lerp(_shootOrigin, target, _t);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && col.TryGetComponent(out Player player))
            {
                player.TakeDamage(damages);
            }
            
            Instantiate(explosionFx);
            _onHitCallBack?.Invoke();
        }
    }
}