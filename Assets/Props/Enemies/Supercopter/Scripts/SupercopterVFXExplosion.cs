using System.Collections;
using Core.VFX;
using UnityEngine;

namespace Props.Enemies.Supercopter
{
    public class SupercopterVFXExplosion : VFXLifeTime
    {
        private CircleCollider2D _explosionCollider;
        
        private void Awake()
        {
            _explosionCollider = GetComponent<CircleCollider2D>();
            _explosionCollider.enabled = false;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(lifeTime - 0.2f);
            _explosionCollider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}