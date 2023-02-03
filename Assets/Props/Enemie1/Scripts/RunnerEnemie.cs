using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RunnerEnemie : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    [Header("Config")]
    public float minSpeed;
    public float maxSpeed;
    public float timingRunMax;
    public float timingIdleMax;

    private float _speed;
    private RunningState _runningState;

    // Start is called before the first frame update
    void Start()
    {
        _runningState = new RunningState(timingRunMax, timingIdleMax);

        _speed = Random.Range(minSpeed, maxSpeed);
    }
    
    private void Update()
    {
        if (null == _target)
            return;
        
        _runningState.AddTiming(Time.deltaTime);

        Vector2 direction = _target.transform.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * (_speed * _runningState.GetSpeed() * Time.deltaTime));
    }
    
    public void SetTarget(GameObject target)
    {
        _target = target;
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

