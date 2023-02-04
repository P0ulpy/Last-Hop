using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

using Prefab = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class Shooter : BaseEnemy
{
    [SerializeField] private Prefab prefabBullet;

    [Header("Config")]
    [SerializeField] private float shootFrequency = 5f;
    [SerializeField] private Vector2 shootFrequencyRange;

    private float _timeSinceLastShoot = 0f;
    private bool _haveProjectile = false;

    // Start is called before the first frame update
    void Start()
    {
        shootFrequency = Random.Range(shootFrequencyRange.x, shootFrequencyRange.y);

        if (_targetTransform != null)
        {
            gameObject.transform.rotation = Utils.GetZRotationFromPositions(transform.position, _targetTransform.position);
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
            Destroy(gameObject);
        }
    }
}
