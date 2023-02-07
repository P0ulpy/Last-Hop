using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTopRoot : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.position.x < transform.position.x)
        {
            _animator.Play("slapLeft");
        }
        else
        {
            _animator.Play("SlapRight");
        }
    }
    
}
