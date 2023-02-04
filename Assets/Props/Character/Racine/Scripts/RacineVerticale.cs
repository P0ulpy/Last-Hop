using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class RacineVerticale : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _rootSprite;


    private bool Stop = false;
    private Vector2 m_startOfRoot = new Vector3(0, 0, 0);
    public float speed = 5;
    public float hauteur = 0;
    private float Spawnx = 0.0f;
    public float offsetY = 1;

    private Vector3 startVector;
    private Vector3 endVector;
    private float time;
    private float duration;


    private void Awake()
    {
        m_startOfRoot = new Vector3(0, (_rootSprite.bounds.size.y / 2) + hauteur, 0);
        transform.position = new Vector3(transform.position.x, -(_rootSprite.bounds.size.y / 2) , 0);

    }

    private void Start()
    {
        startVector = transform.position;
        endVector = new Vector3(transform.position.x, transform.position.y + 18, transform.position.z);
        time = 0;
        duration = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime) , 0);
        transform.position = Vector3.Lerp(startVector, endVector, time / duration);
        time += Time.deltaTime;
    }
    


}