using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private ProgressBarCooldown cooldownShoot;
    [SerializeField] private GameObject rootPrefab;
    
    private GameObject rootPrefabInstance;
    private bool hasShot = false;

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (cooldownShoot.IsInCooldown)return;
            AimWithTheRoot(RacineHorizontale.Direction.Left);
            hasShot = true;
        }

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if(hasShot)
           ShootUpTheRoot();
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (cooldownShoot.IsInCooldown)return;
            AimWithTheRoot(RacineHorizontale.Direction.Right);
            hasShot = true;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if(hasShot)
            ShootUpTheRoot();
        }
    }

    public void ShootUpTheRoot()
    {
        var racineVerticalScript = rootPrefabInstance.GetComponent<RacineHorizontale>();
            racineVerticalScript.StopAimingThenShoot();
            hasShot = false;
    }
    public void AimWithTheRoot(RacineHorizontale.Direction dir)
    {
        cooldownShoot.StartCooldown();
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
