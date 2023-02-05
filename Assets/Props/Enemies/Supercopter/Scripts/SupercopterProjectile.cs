using Core;
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

        private float _t = 0f;

        private Vector3 _shootOrigin;
        private Vector3 _target;
        private UnityAction _onHitCallBack;
        
        private bool _isInShoot = false;
        private bool _isInDeflect = false;

        public void Build(Transform shooterTransform, Transform targetTransform, UnityAction onHit = null)
        {
            _t = 0f;
            
            _shootOrigin = shooterTransform.position;
            _target = targetTransform.position;
            _onHitCallBack = onHit;
            
            _isInShoot = true;
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.O))
                Deflect(transform.position);
#endif
            
            if (!_isInShoot)
                return;
            
            Shoot();
        }

        private void Shoot()
        {
            _t += speed * Time.deltaTime;

            if (_isInDeflect && _t >= 1f)
            {
                Explode();
            }
            
            float curveFactor = curve.Evaluate(_t);

            var target = new Vector2(
                _target.x,
                _target.y + (curveFactor * curveOffset)
            );
            
            transform.position = Vector3.Lerp(_shootOrigin, target, _t);
            transform.LookAt(_target);
        }
        
        private void Deflect(Vector3 deflectPosition)
        {
            _t = 0f;
            speed *= 4;

            float direction = Utils.GetXDirection(_shootOrigin, _target);
            
            _target = new Vector2(
                deflectPosition.x - (3 * direction), 
                _target.y - 1.5f
            );

            _shootOrigin = deflectPosition;
            _isInDeflect = true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && col.TryGetComponent(out Player player))
            {
                player.TakeDamage(damages);
                Explode();
            }
            else if (col.CompareTag("Enemy"))
            {
                Explode();
            }
            else if (col.CompareTag("Deflector"))
            {
                if (_isInDeflect) return;
                
                Deflect(transform.position);
            }
            else if (col.CompareTag("Damager"))
            {
                if (_isInDeflect) return;
                
                Explode();
            }
            else
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            Instantiate(explosionFx, transform.position, Quaternion.identity);
            _onHitCallBack?.Invoke();
        }
    }
}