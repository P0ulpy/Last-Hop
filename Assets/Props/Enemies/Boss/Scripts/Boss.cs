using System.Collections;
using System.Collections.Generic;
using Core;
using Props.Enemies.Supercopter;
using UnityEngine;

using Prefab = UnityEngine.GameObject;

public class Boss : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Prefab bulletPrefab;
    [SerializeField] private Prefab barrilPrefab;
    
    [Header("Points")]
    [SerializeField] private Transform[] shootBulletPoints;
    [SerializeField] private Transform[] shootBarrilPoints;
    
    [Header("Config")]
    [SerializeField] private Vector2 timingShootBulletRange;
    [SerializeField] private Vector2 timingShootBarrilRange;
    [SerializeField] private int health = 700;
    [SerializeField] private int healthForPhaseTired = 400;

    private float _timingShootBulletMax;
    private float _timingShootBarrilMax;
    
    private float _timingShootBullet;
    private float _timingShootBarril;
    
    private Transform target;
    private BossStateMachine.BossStateMachine _bossStateMachine;
    
    // Start is called before the first frame update
    void Start()
    {
        _bossStateMachine = new BossStateMachine.BossStateMachine();
        
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
        
        if(health <= healthForPhaseTired && _bossStateMachine.State == BossStateMachine.EBossState.Normal)
        {
            _bossStateMachine.Update();
        }
        
        if (health <= 0 && _bossStateMachine.State == BossStateMachine.EBossState.Angry)
        {
            _bossStateMachine.Update();
            Die();
        }

        switch (_bossStateMachine.State)
        {
            case BossStateMachine.EBossState.Normal:
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
                break;
            case BossStateMachine.EBossState.Tired:
                SpawnEnemies();
                break;
            case BossStateMachine.EBossState.Angry:
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
                break;
            case BossStateMachine.EBossState.Dead:
                break;
        }
        
        _timingShootBullet += Time.deltaTime;
        _timingShootBarril += Time.deltaTime;

    }

    IEnumerator ShootBullets(bool isAngry = false)
    {
        int nbrShoot = Random.Range(1, shootBulletPoints.Length + (!isAngry ? 1 : 4));
        
        for (int i = 0; i < nbrShoot; i++)
        {
            int shootPos = Random.Range(0, shootBulletPoints.Length);
            var bullet = Instantiate(bulletPrefab, shootBulletPoints[shootPos].position, Quaternion.identity);
            bullet.GetComponent<Bullet>()?.Build(shootBulletPoints[shootPos], target, () => { });
            
            yield return new WaitForSeconds(Random.Range(!isAngry ? 0.25f : 0.1f, !isAngry ? 0.5f : 0.25f));
        }
    }
    
    IEnumerator ShootBarrels(bool isAngry = false)
    {
        int nbrShoot = Random.Range(1, shootBarrilPoints.Length + (!isAngry ? 1 : 4));
        
        for (int i = 0; i < nbrShoot; i++)
        {
            int shootPos = Random.Range(0, shootBarrilPoints.Length);
            var barril = Instantiate(barrilPrefab, shootBarrilPoints[shootPos].position, Quaternion.identity);
            barril.GetComponent<SupercopterProjectile>()?.Build(shootBarrilPoints[shootPos], target, () => { });
            
            yield return new WaitForSeconds(Random.Range(!isAngry ? 0.5f : 0.25f, !isAngry ? 1.5f : 7.25f));
        }
    }

    private void SpawnEnemies()
    {
        return;
    }

    public void TakeDamage(int damage)
    {
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
        public EBossState State = EBossState.Angry;

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


