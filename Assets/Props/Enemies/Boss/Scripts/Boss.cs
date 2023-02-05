using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Props.Enemies.Supercopter;
using UnityEngine;

using Prefab = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    enum EBossSize
    {
        Right,
        Left
    }
    
    [Header("Prefabs")]
    [SerializeField] private Prefab bulletPrefab;
    [SerializeField] private Prefab barrilPrefab;
    
    [Header("Points")]
    [SerializeField] private Transform[] shootBulletPoints;
    [SerializeField] private Transform[] shootBarrilPoints;
    
    [Header("Config")]
    [SerializeField] private int nbrMaxShootBulletNormal;
    [SerializeField] private int nbrMaxShootBarrilNormal;
    [SerializeField] private int nbrMaxShootBulletAngry;
    [SerializeField] private int nbrMaxShootBarrilAngry;
    [SerializeField] private Vector2 timingShootBulletRange;
    [SerializeField] private Vector2 timingShootBarrilRange;
    [SerializeField] private Vector2 timingSwapSizeRange;
    [SerializeField] private int health = 700;
    [SerializeField] private int healthForPhaseTired = 400;
    [SerializeField] private float speed = 1f;
    
    [Header("Timing Config")]
    [SerializeField] private Vector2 timingShootBulletRangeTiredNormal;
    [SerializeField] private Vector2 timingShootBarrilRangeTiredNormal;
    [SerializeField] private Vector2 timingShootBulletRangeTiredAngry;
    [SerializeField] private Vector2 timingShootBarrilRangeTiredAngry;
    
    [Header("Spawn Enemy Config")]
    [SerializeField] private Prefab enemyGroundPrefabs;
    [SerializeField] private Prefab enemyBuldingPrefabs;
    [SerializeField] private Prefab enemySkyPrefabs;
    [SerializeField] private Transform[] spawnPointsGround;
    [SerializeField] private Transform[] spawnPointsBulding;
    [SerializeField] private Transform[] spawnPointsSky;
    [SerializeField] private int nbrEnemySpawnGround;
    [SerializeField] private int nbrEnemySpawnBulding;
    [SerializeField] private int nbrEnemySpawnSky;
    [SerializeField] private int objectifEnemyKill;
    [SerializeField] private Vector2 timingSpawnEnemyRange;

    private int nbrMaxSpawnEnemy => nbrEnemySpawnBulding + nbrEnemySpawnGround + nbrEnemySpawnSky;
    private int enemyKillCount = 0;
    private float _timingSpawnEnemyMax;
    private float _timingSpawnEnemy;



    private float _timingShootBulletMax;
    private float _timingShootBarrilMax;
    
    private float _timingShootBullet;
    private float _timingShootBarril;
    
    private float _timingSwapSizeMax;
    private float _timingSwapSize;
    
    private Transform target;
    private BossStateMachine.BossStateMachine _bossStateMachine = new();
    
    [Header("Deplacement")]
    [SerializeField] private Transform deplacementPointLeft;
    [SerializeField] private Transform deplacementPointRight;
    private EBossSize _bossSize = EBossSize.Left;
    private bool isInMovement = false;
    private Vector3 targetDeplacement;

    // Start is called before the first frame update
    void Start()
    {
        _timingShootBulletMax = Random.Range(timingShootBulletRange.x, timingShootBulletRange.y);
        _timingShootBarrilMax = Random.Range(timingShootBarrilRange.x, timingShootBarrilRange.y);
        
        _timingShootBullet = Random.Range(0, _timingShootBulletMax);
        _timingShootBarril = Random.Range(0, _timingShootBarrilMax);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(target == null)
            target = GameManager.Instance.player.transform;

#if  UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            SwapSize();
        }
#endif

        if (isInMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetDeplacement, speed * Time.deltaTime);

            if (_bossSize == EBossSize.Left)
                DeplacementLeftToRight();
            else
                DeplacementRightToLeft();
            
            return;
        }

        if(health <= healthForPhaseTired && _bossStateMachine.State == BossStateMachine.EBossState.Normal)
        {
            _bossStateMachine.Update();
            ResetAllTimings();
        }
        
        if (health <= 0 && _bossStateMachine.State == BossStateMachine.EBossState.Angry)
        {
            _bossStateMachine.Update();
            ResetAllTimings();
        }
        
        if (_timingSwapSize >= _timingSwapSizeMax)
        {
            SwapSize();
            _timingSwapSizeMax = Random.Range(timingSwapSizeRange.x, timingSwapSizeRange.y);
            _timingSwapSize = 0;
        }

        switch (_bossStateMachine.State)
        {
            case BossStateMachine.EBossState.Normal:
                ShootNormal();
                break;
            case BossStateMachine.EBossState.Tired:
                SpawnEnemies();
                
                if (_timingShootBarril >= _timingShootBarrilMax)
                {
                    StartCoroutine(ShootBarrels());
                    _timingShootBarril = 0;
                    _timingShootBarrilMax = Random.Range(timingShootBarrilRange.x, timingShootBarrilRange.y);
                }
                
                break;
            case BossStateMachine.EBossState.Angry:
                ShootAngry();
                break;
            case BossStateMachine.EBossState.Dead:
                Die();
                break;
        }
        
        _timingShootBullet += Time.deltaTime;
        _timingShootBarril += Time.deltaTime;
        _timingSwapSize += Time.deltaTime;
    }
    
    private void ResetAllTimings()
    {
        _timingShootBullet = 0;
        _timingShootBarril = 0;
        _timingSwapSize = 0;
    }

    private void DeplacementLeftToRight()
    {
        if (transform.position != targetDeplacement)
            return;
                
        if (targetDeplacement == deplacementPointLeft.position)
        {
            var position = deplacementPointRight.position;
            transform.position = position;
            transform.Rotate(0 , 180, 0);
            targetDeplacement = position + new Vector3(-10.0f, 0, 0);
        }
        else if (targetDeplacement == deplacementPointRight.position + new Vector3(-10.0f, 0, 0))
        {
            _bossSize = EBossSize.Right;
            isInMovement = false;
        }
    }
    
    private void DeplacementRightToLeft()
    {
        if (transform.position != targetDeplacement)
            return;
                
        if (targetDeplacement == deplacementPointRight.position)
        {
            var position = deplacementPointLeft.position;
            transform.position = position;
            transform.Rotate(0 , -180, 0);
            targetDeplacement = position + new Vector3(10.0f, 0, 0);
        }
        else if (targetDeplacement == deplacementPointLeft.position + new Vector3(10.0f, 0, 0))
        {
            _bossSize = EBossSize.Left;
            isInMovement = false;
        }
    }

    private void ShootNormal()
    {
        if (_timingShootBullet >= _timingShootBulletMax)
        {
            StartCoroutine(ShootBullets());
            _timingShootBullet = 0;
            _timingShootBulletMax = Random.Range(timingShootBulletRange.x, timingShootBulletRange.y);
        }

        if (_timingShootBarril >= _timingShootBarrilMax)
        {
            StartCoroutine(ShootBarrels());
            _timingShootBarril = 0;
            _timingShootBarrilMax = Random.Range(timingShootBarrilRange.x, timingShootBarrilRange.y);
        }
    }
    
    private void ShootAngry()
    {
        if (_timingShootBullet >= _timingShootBulletMax)
        {
            StartCoroutine(ShootBullets(true));
            _timingShootBullet = 0;
            _timingShootBulletMax = Random.Range(timingShootBulletRange.x / 2, timingShootBulletRange.y / 2);
        }

        if (_timingShootBarril >= _timingShootBarrilMax)
        {
            StartCoroutine(ShootBarrels(true));
            _timingShootBarril = 0;
            _timingShootBarrilMax = Random.Range(timingShootBarrilRange.x / 2, timingShootBarrilRange.y / 2);
        }
    }

    IEnumerator ShootBullets(bool isAngry = false)
    {
        int nbrShoot = Random.Range(1, (!isAngry ? nbrMaxShootBulletNormal : nbrMaxShootBulletAngry));
        
        for (int i = 0; i < nbrShoot; i++)
        {
            int shootPos = Random.Range(0, shootBulletPoints.Length);
            var bullet = Instantiate(bulletPrefab, shootBulletPoints[shootPos].position, Quaternion.identity);
            bullet.GetComponent<Bullet>()?.Build(shootBulletPoints[shootPos], target, () =>
            {
                Destroy(bullet.gameObject);
            });
            
            yield return new WaitForSeconds(
                Random.Range(
                    !isAngry ? timingShootBulletRangeTiredNormal.x : timingShootBulletRangeTiredAngry.x, 
                    !isAngry ? timingShootBulletRangeTiredNormal.y : timingShootBulletRangeTiredAngry.y
                    )
                );
        }
    }
    
    IEnumerator ShootBarrels(bool isAngry = false)
    {
        int nbrShoot = Random.Range(1, (!isAngry ? nbrMaxShootBarrilNormal : nbrMaxShootBarrilAngry));
        
        for (int i = 0; i < nbrShoot; i++)
        {
            int shootPos = Random.Range(0, shootBarrilPoints.Length);
            var barril = Instantiate(barrilPrefab, shootBarrilPoints[shootPos].position, Quaternion.identity);
            barril.GetComponent<SupercopterProjectile>()?.Build(shootBarrilPoints[shootPos], target, () =>
            {
                Destroy(barril.gameObject);
            });
            
            yield return new WaitForSeconds(
                Random.Range(
                    !isAngry ? timingShootBarrilRangeTiredNormal.x : timingShootBarrilRangeTiredAngry.x, 
                    !isAngry ? timingShootBarrilRangeTiredNormal.y : timingShootBarrilRangeTiredAngry.y
                    )
                );
        }
    }

    private void SpawnEnemies()
    {
        if (objectifEnemyKill <= enemyKillCount)
        {
            _bossStateMachine.Update();
            ResetAllTimings();
            return;
        }
        
        _timingSpawnEnemy += Time.deltaTime;
        
        if(nbrMaxSpawnEnemy <= 0)
            return;

        if (_timingSpawnEnemy < _timingSpawnEnemyMax)
            return;
            
        int typeEnemyToSpawn = Random.Range(1, 4);

        bool isSpawn = false;
        if (typeEnemyToSpawn == 1)
        {
            isSpawn = SpawnEnemy(enemyGroundPrefabs, spawnPointsGround, 1);
        }
        
        if(typeEnemyToSpawn == 2 || (typeEnemyToSpawn == 1 && isSpawn == false))
        {
            isSpawn = SpawnEnemy(enemyBuldingPrefabs, spawnPointsBulding, 2);
        }
        
        if(typeEnemyToSpawn == 3 || isSpawn == false)
        {
            SpawnEnemy(enemySkyPrefabs, spawnPointsSky, 3);
        }
        
        return;
    }

    private bool SpawnEnemy(GameObject prefab, Transform[] spawnPoint, int i)
    {
        switch(i)
        {
            case 1:
                if (nbrEnemySpawnGround == 0)
                    return false;
                break;
            case 2:
                if (nbrEnemySpawnBulding == 0)
                    return false;
                break;
            case 3:
                if (nbrEnemySpawnSky == 0)
                    return false;
                break;
        }
        
        int spawnPos = Random.Range(0, spawnPoint.Length);
        var enemy = Instantiate(prefab, spawnPoint[spawnPos].position, Quaternion.identity);
        
        enemy.GetComponent<BaseEnemy>()?.SetTarget(GameManager.Instance.player.transform);

        switch (i)
        {
            case 1:
                enemy.GetComponent<RunnerEnemie>()?.OnExplode(() =>
                {
                    enemyKillCount++;
                });
                nbrEnemySpawnGround--;
                break;
            case 2:
                enemy.GetComponent<Shooter>()?.OnExplode(() =>
                {
                    enemyKillCount++;
                });
                nbrEnemySpawnBulding--;
                break;
            case 3:
                enemy.GetComponent<Supercopter>()?.OnExplode(() =>
                {
                    enemyKillCount++;
                });
                nbrEnemySpawnSky--;
                break;
        }
        
        return true;
    }

    private void SwapSize()
    {
        switch (_bossSize)
{
            case EBossSize.Left:
                targetDeplacement = deplacementPointLeft.position;
                break;
            case EBossSize.Right:
                targetDeplacement = deplacementPointRight.position;
                break;
        }
        
        isInMovement = true;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(_bossStateMachine.State);
        
        if(_bossStateMachine.State == BossStateMachine.EBossState.Dead && _bossStateMachine.State == BossStateMachine.EBossState.Tired)
            return;
        
        health -= damage;
    }

    private void Die()
    {
        throw new System.NotImplementedException();
    }
}

namespace BossStateMachine
{
    enum EBossState
    {
        Normal,
        Tired,
        Angry,
        Dead
    }
    
    

    interface IBossState
    {
        public IBossState Update(BossStateMachine runningState);
    }
    
    class NormalState : IBossState
    {
        public IBossState Update(BossStateMachine runningState)
        {
            runningState.State = EBossState.Tired;
            return new TiredState();
        }
    }
    
    class TiredState : IBossState
    {
        public IBossState Update(BossStateMachine runningState)
        {
            runningState.State = EBossState.Angry;
            return new AngryState();
        }
    }
    
    class AngryState : IBossState
    {
        public IBossState Update(BossStateMachine runningState)
        {
            runningState.State = EBossState.Dead;
            return new DeadState();
        }
    }
    
    class DeadState : IBossState
    {
        public IBossState Update(BossStateMachine runningState)
        {
            return this;
        }
    }
    
    class BossStateMachine
    {
        private IBossState _currentState;
        public EBossState State = EBossState.Normal;

        public BossStateMachine()
        {
            _currentState = new NormalState();
        }

        public void Update()
        {
            _currentState = _currentState.Update(this);
        }
    }
}


