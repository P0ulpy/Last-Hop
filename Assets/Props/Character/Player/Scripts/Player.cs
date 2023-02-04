using System.Collections.Generic;
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
    
    
    private GameObject rootPrefabInstance;
    private bool hasShot = false;
    
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
            hasShot = true;
        }

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if(hasShot)
           ShootUpTheRoot();
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (cooldownShootRight.IsInCooldown)return;
            AimWithTheRoot(RacineHorizontale.Direction.Right);
            hasShot = true;
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            if(hasShot)
            ShootUpTheRoot();
        }
        
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartRegen();
        }

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

    public void ShootUpTheRoot()
    {
        var racineVerticalScript = rootPrefabInstance.GetComponent<RacineHorizontale>();
            racineVerticalScript.StopAimingThenShoot();
            hasShot = false;
    }
    public void AimWithTheRoot(RacineHorizontale.Direction dir)
    {
        if (dir == RacineHorizontale.Direction.Left) cooldownShootLeft.StartCooldown();
        else if (dir == RacineHorizontale.Direction.Right) cooldownShootRight.StartCooldown();

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
