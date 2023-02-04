using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter2D");
        
        if(col.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
            TakeDamage();
        else if(col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            TakeDamage(20);
        
        Destroy(col.gameObject);
    }

    private void TakeDamage(int damage = 10)
    {
        healthBar.CurrentVal -= damage;
        if (healthBar.CurrentVal <= 0)
            Destroy(gameObject);
    }
}
