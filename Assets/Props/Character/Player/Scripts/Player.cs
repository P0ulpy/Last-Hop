using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private ProgressBarCooldown cooldownShootRight;
    [SerializeField] private ProgressBarCooldown cooldownShootLeft;
    [SerializeField] private GameObject rootPrefab;
    [SerializeField] private Animator _animator;
    [Header("Regen")]
    [SerializeField] private float fxStaticTimeMax = 3f;
    [SerializeField] private float fxTravelTimeMax = 1f;
    [SerializeField] private GameObject regenPrefab;
    [SerializeField] private Transform[] regenSpawnPoints;
    private readonly List<GameObject> fxSpawn = new ();
    private bool isRegen = false;
    private float fxStaticTime = 0f;
    private float fxTravelTime = 0f;
    private float rotationAfterHit;
    
    private GameObject rootPrefabInstanceLeft;
    private GameObject rootPrefabInstanceRight;
    private bool hasShotLeft = false;
    private bool hasShotRight = false;
    private bool cantTakeDamage = false;
    
    private static readonly int DamageTaken = Animator.StringToHash("damageTaken");

    private void Update()
    {
        DispatchInputs();

        if (isRegen)
            UpdateRegen();
    }

    private void DispatchInputs()
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
    }

    private void UpdateRegen()
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

    public void ShootUpTheRoot(RacineHorizontale.Direction dir)
    {
        
        switch (dir)
        {
            case RacineHorizontale.Direction.Left:
            {
                _animator.SetTrigger("endHoriLeft");
                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotLeft = false;
               // transform.Rotate(new Vector3(0,180,0));
                racineVerticalScript.OnEndSpell += () =>
                {
                    //_animator.SetBool("Attack",false);
                    //_animator.SetBool("endHori",false);
                    _animator.SetTrigger("endRootUpLeft");
                    
                };
            } break;
            case RacineHorizontale.Direction.Right:
            {
                _animator.SetTrigger("endHori");
                var racineVerticalScript = rootPrefabInstanceRight.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotRight = false;
                racineVerticalScript.OnEndSpell += () =>
                {
                    //_animator.SetBool("Attack",false);
                    //_animator.SetBool("endHori",false);
                    _animator.SetTrigger("endRootUp");
                };
            } break;
        }
    }
    public void AimWithTheRoot(RacineHorizontale.Direction dir)
    {

        switch (dir)
        {
            case RacineHorizontale.Direction.Left:
            {
                _animator.SetTrigger("AttackLeft");
                cooldownShootLeft.StartCooldown();
                rootPrefabInstanceLeft = Instantiate(rootPrefab);

                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StartAiming(dir);
                // transform.Rotate(new Vector3(0,180,0));
            } break;
            case RacineHorizontale.Direction.Right:
            {
                _animator.SetTrigger("Attack");
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
        
        if (healthBar.CurrentVal <= 0)
        {
            OnDeath();
            _animator.SetBool("isDead",true);
        }

        if(!cantTakeDamage)
            StartCoroutine(DamageSpriteForSeconds());
    }
    
    IEnumerator DamageSpriteForSeconds()
    {
        cantTakeDamage = true;
        _animator.SetBool(DamageTaken, true);
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool(DamageTaken, false);
        cantTakeDamage = false;
    }
    

    public void StartRegen()
    {
        isRegen = true;
        foreach (var spawnPoint in regenSpawnPoints)
        {
            fxSpawn.Add(
                Instantiate(regenPrefab, spawnPoint.position, Quaternion.identity)
            );
        }
    }

    private void OnDeath()
    {
        
        Cleanup();
    }

    private void Cleanup()
    {
        foreach(var fx in fxSpawn)
            Destroy(fx);
    }
}
