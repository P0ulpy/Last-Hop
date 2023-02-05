using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private ProgressBarCooldown cooldownShootRight;
    [SerializeField] private ProgressBarCooldown cooldownShootLeft;
    [SerializeField] private GameObject rootPrefab;
    
    [Header("Regen")]
    [SerializeField] private float fxStaticTimeMax = 3f;
    [SerializeField] private float fxTravelTimeMax = 1f;
    [SerializeField] private GameObject regenPrefab;
    [SerializeField] private Transform[] regenSpawnPoints;
    private List<GameObject> fxSpawn;
    private bool isRegen = false;
    private float fxStaticTime = 0f;
    private float fxTravelTime = 0f;
    
    
    private GameObject rootPrefabInstanceLeft;
    private GameObject rootPrefabInstanceRight;
    private bool hasShotLeft = false;
    private bool hasShotRight = false;
    
    
    private void Start()
    {
        fxSpawn = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (cooldownShootLeft.IsInCooldown)return;
            AimWithTheRoot(RacineHorizontale.Direction.Left);
            hasShotLeft = true;
        }

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if(hasShotLeft)
           ShootUpTheRoot(RacineHorizontale.Direction.Left);
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (cooldownShootRight.IsInCooldown)return;
            AimWithTheRoot(RacineHorizontale.Direction.Right);
            hasShotRight = true;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if(hasShotRight)
            ShootUpTheRoot(RacineHorizontale.Direction.Right);
        }
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartRegen();
        }
#endif

        if (isRegen)
        {
            if(fxStaticTime < fxStaticTimeMax)
                fxStaticTime += Time.deltaTime;
            else
            {
                if(fxTravelTime < fxTravelTimeMax)
                {
                    fxTravelTime += Time.deltaTime;
                    foreach (var fx in fxSpawn)
                    {
                        fx.transform.position = Vector3.MoveTowards(fx.transform.position, transform.position, Time.deltaTime);
                    }
                }
                else
                {
                    isRegen = false;
                    fxStaticTime = 0f;
                    fxTravelTime = 0f;
                    foreach (var fx in fxSpawn)
                    {
                        Destroy(fx);
                    }
                    fxSpawn.Clear();

                    healthBar.CurrentVal += healthBar.MaxValue * 0.33f;
                }
            }
        }
    }

    public void ShootUpTheRoot(RacineHorizontale.Direction dir)
    {
        switch (dir)
        {
            case RacineHorizontale.Direction.Left:
            {
                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotLeft = false;
            } break;
            case RacineHorizontale.Direction.Right:
            {
                var racineVerticalScript = rootPrefabInstanceRight.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotRight = false;
            } break;
        }

                
    }
    public void AimWithTheRoot(RacineHorizontale.Direction dir)
    {

        switch (dir)
        {
            case RacineHorizontale.Direction.Left:
            {
                cooldownShootLeft.StartCooldown();
                rootPrefabInstanceLeft = Instantiate(rootPrefab);
                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StartAiming(dir);
            } break;
            case RacineHorizontale.Direction.Right:
            {
                cooldownShootRight.StartCooldown();
                rootPrefabInstanceRight = Instantiate(rootPrefab);
                var racineVerticalScript = rootPrefabInstanceRight.GetComponent<RacineHorizontale>();
                racineVerticalScript.StartAiming(dir);
            } break;
        }

        
    }
    public void TakeDamage(int damage = 10)
    {
        healthBar.CurrentVal -= damage;
        //if (healthBar.CurrentVal <= 0)
            //Destroy(gameObject);
    }

    public void StartRegen()
    {
        isRegen = true;
        foreach (var spawnPoint in regenSpawnPoints)
        {
            var fx = Instantiate(regenPrefab, spawnPoint.position, Quaternion.identity);
            fxSpawn.Add(fx);
        }
    }
}
