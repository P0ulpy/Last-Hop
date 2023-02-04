using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class RacineVerticale : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _rootSprite;

    private bool Start = true;
    private bool Stop = false;
    private Vector2 m_startOfRoot = new Vector3(0, 0, 0);
    public float speed = 5;
    public float hauteur = 0;
    private float Spawnx = 0.0f;
    public float offsetY = 1;


    private void Awake()
    {
        m_startOfRoot = new Vector3(0, (_rootSprite.bounds.size.y / 2) + hauteur, 0);
        transform.position = new Vector3(transform.position.x, -(_rootSprite.bounds.size.y / 2) , 0);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime) , 0);
    }
    


}