using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [Header("Config")]
    [SerializeField] private float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }
    
    private void UpdatePosition()
    {
        var position = transform.position;
        var direction = (targetTransform.position - position).normalized;
        transform.position = position + (direction * (speed * Time.deltaTime));
    }
    
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }
}
