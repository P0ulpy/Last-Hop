using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class RacineHorizontale : MonoBehaviour
{
     public enum Direction {Right, Left};
    [SerializeField] private GameObject racineVerticale;
    [SerializeField] private SpriteRenderer _rootSprite;
    [SerializeField] private GameObject _spriteMask;

    private int sideSign = 0;
    private float offset = 0;
    private bool Start = true;
    private bool Stop = false;
    private Vector2 m_startOfRoot = new Vector3(0, 0, 0);
    public float speed = 6;
    public float hauteur = -1;
    
    // Update is called once per frame
    void Update()
    {
        if (Start)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            { 
                StartAiming(Direction.Left); 
                Start = false;
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartAiming(Direction.Right);
                Start = false;
            } 
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAimingThenShoot();
            Start = false;
        }

        if (!Stop) _spriteMask.transform.localPosition = new Vector3(_spriteMask.transform.localPosition.x + (sideSign * speed * Time.deltaTime), 0, 0);


    }


    public void StopAimingThenShoot()
    {
        Stop = true;
        
        Instantiate(racineVerticale, new Vector3(offset + _spriteMask.transform.position.x,hauteur,0) , Quaternion.identity);
    }

    public void StartAiming(Direction myDir)
    {
        if (myDir == Direction.Right)
        {
            m_startOfRoot = new Vector3((_rootSprite.bounds.size.x / 2), hauteur, 0);
            sideSign = 1;
            offset = -_rootSprite.bounds.size.x;
        }
        else if(myDir == Direction.Left)
        {
            sideSign = -1;
            m_startOfRoot = new Vector3(-(_rootSprite.bounds.size.x / 2), hauteur, 0);
            offset = 0;
        }
        
        
        _spriteMask.transform.localPosition = sideSign*m_startOfRoot;
        this.transform.position = new Vector3(m_startOfRoot.x,hauteur,0); 
    }

    
    
}
