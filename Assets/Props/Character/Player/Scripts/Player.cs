using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private static readonly int AttackLeft = Animator.StringToHash("AttackLeft");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int EndRootUp = Animator.StringToHash("endRootUp");
    private static readonly int EndHori = Animator.StringToHash("endHori");
    private static readonly int EndRootUpLeft = Animator.StringToHash("endRootUpLeft");
    private static readonly int EndHoriLeft = Animator.StringToHash("endHoriLeft");
    private static readonly int IsDead = Animator.StringToHash("isDead");

    private SpriteTintDamage _spriteTintDamage;

    private int nbrOfAnim = 0;

    private void Awake()
    {
        _spriteTintDamage = GetComponent<SpriteTintDamage>();
    }

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
                nbrOfAnim--;
                _animator.Play("leftUPattack");
                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotLeft = false;
                racineVerticalScript.OnEndSpell += () =>
                {
                    if(nbrOfAnim == 0)
                    _animator.Play("idle");
                    
                };
            } break;
            case RacineHorizontale.Direction.Right:
            {
                nbrOfAnim--;
                _animator.Play("rootUp");
                var racineVerticalScript = rootPrefabInstanceRight.GetComponent<RacineHorizontale>();
                racineVerticalScript.StopAimingThenShoot();
                hasShotRight = false;
                racineVerticalScript.OnEndSpell += () =>
                {
                    if(nbrOfAnim == 0)
                    _animator.Play("idle");
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
                
               // _animator.SetTrigger(AttackLeft);
               _animator.Play("leftRootAvance");
               nbrOfAnim++;
                cooldownShootLeft.StartCooldown();
                rootPrefabInstanceLeft = Instantiate(rootPrefab);

                var racineVerticalScript = rootPrefabInstanceLeft.GetComponent<RacineHorizontale>();
                racineVerticalScript.StartAiming(dir);
                // transform.Rotate(new Vector3(0,180,0));
            } break;
            case RacineHorizontale.Direction.Right:
            {
                nbrOfAnim++;
                //_animator.SetTrigger(Attack);
                _animator.Play("rootavance");
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
            //_animator.SetBool(IsDead,true);
            _animator.Play("death");
        }

        if(!cantTakeDamage)
            StartCoroutine(DamageSpriteForSeconds());
    }
    
    IEnumerator DamageSpriteForSeconds()
    {
        cantTakeDamage = true;
       // _animator.SetBool(DamageTaken, true);
        _animator.Play("damage");
        _spriteTintDamage.StartTint();
        Core.GameManager.Instance.PlaySoundPlayerDamage();
        yield return new WaitForSeconds(0.3f);
        //_animator.SetBool(DamageTaken, false);
        _animator.Play("idle");
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
        SceneManager.LoadScene("MenuLose");
    }

    private void Cleanup()
    {
        foreach(var fx in fxSpawn)
            Destroy(fx);
    }
}
