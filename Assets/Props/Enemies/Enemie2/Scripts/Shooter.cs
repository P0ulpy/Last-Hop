using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Events;
using Prefab = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class Shooter : BaseEnemy
{
    [SerializeField] private Prefab prefabBullet;

    [Header("Config")]
    [SerializeField] private float shootFrequency = 5f;
    [SerializeField] private Vector2 shootFrequencyRange;
    
    private UnityAction _onHitCallBack;
    [SerializeField] private GameObject explosionFx;

    private float _timeSinceLastShoot = 0f;
    private bool _haveProjectile = false;

    // Start is called before the first frame update
    void Start()
    {
        shootFrequency = Random.Range(shootFrequencyRange.x, shootFrequencyRange.y);

        if (_targetTransform != null)
        {
            if(Utils.GetXDirection(transform.position, _targetTransform.position) == -1)
                gameObject.GetComponent<SpriteRenderer>().flipY = true;
            transform.right = _targetTransform.position - transform.position;
        }
    }

    private void Update()
    {
        if (_targetTransform == null)
            return;
        
        if(_timeSinceLastShoot <= shootFrequency)
            _timeSinceLastShoot += Time.deltaTime;
        else if(!_haveProjectile)
            Shoot();
    }

    private void Shoot()
    {
        _haveProjectile = true;
        
        var bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Build(transform, _targetTransform, () =>
        {
            _haveProjectile = false;
            _timeSinceLastShoot = 0f;
            Destroy(bullet.gameObject);
        });
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Damager"))
        {
            Explode();
            Destroy(gameObject);
        }
    }
    
    public void OnExplode(UnityAction onHit)
    {
        _onHitCallBack = onHit;
    }

    public void Explode()
    {
        if(explosionFx != null)
            Instantiate(explosionFx, transform.position, Quaternion.identity);
        _onHitCallBack?.Invoke();
    }
}
