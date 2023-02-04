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
    
    private Coroutine _shootCoroutine;
    private bool IsShooting = false;

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
        if (!IsShooting)
        {
            IsShooting = true;
            _shootCoroutine = StartCoroutine(Shoot());
        }
    }

    protected override void OnDestroy() 
    {

        base.OnDestroy();

        StopCoroutine(_shootCoroutine);
    }

    IEnumerator Shoot()
    {
        if (_targetTransform == null)
            yield return Shoot();
        
        var bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Build(transform, _targetTransform);

        yield return new WaitForSeconds(shootFrequency);
        yield return Shoot();
    }
}
