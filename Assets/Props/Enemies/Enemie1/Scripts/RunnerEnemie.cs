using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RunnerEnemie : BaseEnemy
{
    [Header("Config")] 
    [SerializeField] private int damage = 20;
    public Vector2 speedRange;
    public Vector2 timingRunRange;
    public Vector2 timingIdleRange;
    public Animator _animator;
    
    private float _speed;
    private RunningState _runningState;
    private bool hitPlayer = false;

    // Start is called before the first frame update
    private void Awake()
    {
        _runningState = new RunningState(Random.Range(timingRunRange.x, timingRunRange.y), Random.Range(timingIdleRange.x, timingIdleRange.y));
    }

    void Start()
    {
        _speed = Random.Range(speedRange.x, speedRange.y);
    }
    
    private void Update()
    {

        if (hitPlayer)
        {
            //StartCoroutine(ApplyDamageDelay(player));
            return;
        }
        if (null == _targetTransform)
            return;
        _runningState.AddTiming(Time.deltaTime);
        _animator.SetFloat("speed",_runningState.GetSpeed());
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var position = transform.position;
        var xDirection = Utils.GetXDirection(position, _targetTransform.position);
        var newX = position.x + (xDirection * (_speed * _runningState.GetSpeed() * Time.deltaTime));
            
        transform.position = new Vector2(newX, position.y);
        
        transform.rotation = Utils.GetYRotationFromXDirection(xDirection, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && col.TryGetComponent(out Player player))
        {
            hitPlayer = true;
            _animator.SetBool("hitPlayer",true);
            StartCoroutine(ApplyDamageDelay(player));
        }
        
        if (col.CompareTag("Damager"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ApplyDamageDelay(Player player)
    {
        player.TakeDamage(transform.position,damage);
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2);
        GetComponent<Collider2D>().enabled = true;
    }
}

enum EStateRunning
{
    Run,
    Idle
}

class RunningState
{
    private IStateRunning _stateRunning;
    public EStateRunning State;
    
    private float timingRunMax;
    private float timingIdleMax;
    
    private float timing;

    public RunningState(float timingRun, float timingIdle)
    {
        timingIdleMax = timingIdle;
        timingRunMax = timingRun;
        
        _stateRunning = new Run();
        State = EStateRunning.Run;
        
    }

    public void AddTiming(float time)
    {
        timing += time;
        if (State == EStateRunning.Run)
        {
            if (!(timing >= timingRunMax)) return;
            
            ChangeState();
            timing = 0;
        }
        else
        {
            if (!(timing >= timingIdleMax)) return;
            ChangeState();
            timing = 0;
        }
    }

    public void ChangeState()
    {
        _stateRunning = _stateRunning.ChangeState(this);
    }
    
    public float GetSpeed()
    {
        return _stateRunning.GetSpeed();
    }
}

class Run : IStateRunning
{
    public Run()
    {
        _speed = 1;
    }
    
    public override IStateRunning ChangeState(RunningState runningState)
    {
        runningState.State = EStateRunning.Idle;
        return new Idle();
    }
}

class Idle : IStateRunning
{
    public Idle()
    {
        _speed = 0;
    }
    
    public override IStateRunning ChangeState(RunningState runningState)
    {
        runningState.State = EStateRunning.Run;
        return new Run();
    }
}

abstract class IStateRunning
{
    protected float _speed;
    public abstract IStateRunning ChangeState(RunningState runningState);
    
    public float GetSpeed()
    {
        return _speed;
    }
}

