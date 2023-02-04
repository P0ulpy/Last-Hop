using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;

    
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
        
        Destroy(col.gameObject);
    }

    public void TakeDamage(int damage = 10)
    {
        healthBar.CurrentVal -= damage;
        //if (healthBar.CurrentVal <= 0)
            //Destroy(gameObject);
    }
}
