using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private GameObject rootPrefab;
    private GameObject rootPrefabInstance;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            //if(col.CompareTag("SupercopterProjectile"))
                //TakeDamage(20);
            if (col.CompareTag("ShooterProjectile"))
                TakeDamage(10);
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            TakeDamage(15);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AimWithTheRoot(RacineHorizontale.Direction.Left);
        }

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            ShootUpTheRoot();
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            AimWithTheRoot(RacineHorizontale.Direction.Right);
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            ShootUpTheRoot();
        }

    }

    public void ShootUpTheRoot()
    {
            var racineVerticalScript = rootPrefabInstance.GetComponent<RacineHorizontale>();
            racineVerticalScript.StopAimingThenShoot();
    }
    public void AimWithTheRoot(RacineHorizontale.Direction dir)
    {
            rootPrefabInstance = Instantiate(rootPrefab);
            var racineVerticalScript = rootPrefabInstance.GetComponent<RacineHorizontale>();
            racineVerticalScript.StartAiming(dir);
    }
    public void TakeDamage(int damage = 10)
    {
        healthBar.CurrentVal -= damage;
        //if (healthBar.CurrentVal <= 0)
            //Destroy(gameObject);
    }
}
