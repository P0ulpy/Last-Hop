using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

using Prefab = UnityEngine.GameObject;

public class Shooter : BaseEnemy
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Prefab prefabBullet;
    
    [Header("Config")]
    [SerializeField] private float shootFrequency = 5f;
    
    private Coroutine _shootCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _shootCoroutine = StartCoroutine(Shoot());
        
        if(targetTransform != null)
            gameObject.transform.rotation = Utils.GetZRotationFromPositions(transform.position, targetTransform.position);
    }

    private void OnDestroy()
    {
        StopCoroutine(_shootCoroutine);
    }

    IEnumerator Shoot()
    {
        if (targetTransform == null)
            yield return Shoot();
        
        var bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetTarget(targetTransform);

        yield return new WaitForSeconds(shootFrequency);
        yield return Shoot();
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
        gameObject.transform.LookAt(targetTransform);
    }
}
