using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx;
    
    private Vector3 _shootOrigin;
    private Vector3 _target;
    private UnityAction _onHitCallBack;

    [Header("Config")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damages = 10;
    
    private bool _isInDeflect = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void Build(Transform shooterTransform, Transform targetTransform, UnityAction onHit = null)
    {
        _shootOrigin = shooterTransform.position;
        _target = targetTransform.position;
        _onHitCallBack = onHit;
        transform.right = _target - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
            Deflect();
#endif
        
        UpdatePosition();
    }
    
    private void UpdatePosition()
    {
        var position = transform.position;
        var direction = (_target - position).normalized;
        transform.position = position + (direction * (speed * Time.deltaTime));
    }

    public void Deflect()
    {
        Core.GameManager.Instance.PlaySoundSwoosh();
        
        gameObject.GetComponent<CircleCollider2D>().isTrigger = true;

        _target = _shootOrigin;
        _isInDeflect = true;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && col.TryGetComponent(out Player player))
        {
            player.TakeDamage(damages);
            Explode();
        }
        else if(col.CompareTag("Boss") && col.TryGetComponent(out Boss boss))
        {
            if (!_isInDeflect) return;
            
            boss.TakeDamage(damages);
            Explode();
        }
        else if (col.CompareTag("Deflector"))
        {
            if (_isInDeflect) return;
            Deflect();
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
