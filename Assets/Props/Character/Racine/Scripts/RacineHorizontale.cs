using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public class RacineHorizontale : MonoBehaviour
{
     public enum Direction {Right, Left};
    [SerializeField] private GameObject racineVerticale;
    [SerializeField] private SpriteRenderer _rootSprite;
    [SerializeField] private GameObject _Mask;

    private int sideSign = 0;
    private float offset = 0;
    private bool begin = true;
    private bool Stop = false;
    private Vector3 m_startOfRoot = new Vector3(0, 0, 0);
    public float speed = 6;
    public float hauteur = -1;

    private Vector2 screensBounds;
    private float startPositionX;
    
    //temp
    private bool canMoveNow = false;
    // Update is called once per frame

    private void Awake()
    {
        screensBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
       
    }

    private void LateUpdate()
    {

    }

    void Update()
    {
        if (begin)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            { 
                StartAiming(Direction.Left); 
                begin = false;
                canMoveNow = true;
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartAiming(Direction.Right);
                begin = false;
                canMoveNow = true;
            } 
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAimingThenShoot();
            begin = false;
        }
        
        
        if (!Stop && canMoveNow) Move();
    }

    public void Move()
    {
        _Mask.transform.localPosition = new Vector3(_Mask.transform.localPosition.x + (sideSign * speed * Time.deltaTime), 0, 0);

        if (Mathf.Abs(_Mask.transform.position.x) > Mathf.Abs(startPositionX + screensBounds.x))
        {
            StopAimingThenShoot();
        }
    }
    public void StopAimingThenShoot()
    {
        Stop = true;
        
        Instantiate(racineVerticale, new Vector3(_Mask.transform.position.x ,hauteur,0) , Quaternion.identity);
    }

    public void StartAiming(Direction myDir)
    {
        if (myDir == Direction.Right)
        {
            m_startOfRoot = new Vector3((_rootSprite.bounds.size.x / 2), hauteur, 0);
            offset = -_rootSprite.bounds.size.x;
            sideSign = 1;
        }
        else if(myDir == Direction.Left)
        {
            m_startOfRoot = new Vector3(-(_rootSprite.bounds.size.x / 2), hauteur, 0);
            offset = 0;
            sideSign = -1;
            _Mask.transform.Rotate(new Vector3(0,0,180));
            _Mask.transform.localPosition =
                new Vector3(_Mask.transform.localPosition.x + _rootSprite.bounds.size.x, hauteur, 0);
        }
        this.transform.position = new Vector3(m_startOfRoot.x,hauteur,0); 
        startPositionX = _Mask.transform.localPosition.x + transform.position.x;
    }

    
    
}
