using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class RacineVerticale : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _rootSprite;


    public float duration;
    private bool stopUpdate = false;
    private enum Direction {Up, Down};
    private Vector3 startVector;
    private Vector3 endVector;
    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private float time;
    private bool hasReset = false;
    private Vector2 screensBounds;

    [SerializeField] private Animator _animator;
    
    
    

    public event UnityAction OnRetract;
    
    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, -(_rootSprite.bounds.size.y / 2) , 0);
        firstPosition = transform.position;
        screensBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Initialise(Direction.Up);
        
        //Spawn Virage
        
    }
    
    
    
    
    private void Initialise(Direction dir)
    {
        
        if (dir == Direction.Up)
        {
            startVector = transform.position ;
            endVector = new Vector3(transform.position.x, (Mathf.Abs(screensBounds.y) - _rootSprite.bounds.size.y / 2 ), transform.position.z);
        }
        if(dir == Direction.Down)
        {
            startVector = lastPosition ;
            endVector = firstPosition;
        }
        
        time = 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (stopUpdate) return;
        
       // float newY = Mathf.SmoothStep(startVector.y, endVector.y, time / duration);

       // transform.position = new Vector3(startVector.x, newY, startVector.z);
        transform.position = Vector3.Lerp(startVector, endVector, time/duration);
        time += Time.deltaTime;
        if (time >= duration)
        {
            if (hasReset)End();
            Reset();
        }
    }

    private void End()
    {
        stopUpdate = true;
        OnRetract?.Invoke();
    }
    

    private void Reset()
    {
        lastPosition = transform.position;
        Initialise(Direction.Down);
        hasReset = true;
    }


}