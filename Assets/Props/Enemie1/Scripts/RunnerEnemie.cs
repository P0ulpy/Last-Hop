using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RunnerEnemie : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [Header("Config")] 
    public Vector2 speedRange;
    public Vector2 timingRunRange;
    public Vector2 timingIdleRange;

    private float _speed;
    private RunningState _runningState;

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
        if (null == targetTransform)
            return;
        
        _runningState.AddTiming(Time.deltaTime);

        UpdatePosition();
    }
    
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }
    
    private void UpdatePosition()
    {
        var position = transform.position;
        var xDirection = Utils.GetXDirection(position, targetTransform.position);
        var newX = position.x + (xDirection * (_speed * _runningState.GetSpeed() * Time.deltaTime));
            
        transform.position = new Vector2(newX, position.y);
        
        transform.rotation = Utils.GetYRotationFromXDirection(xDirection, transform.rotation);
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

