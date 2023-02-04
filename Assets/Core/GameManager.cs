using System;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private Transform _playerTransform;

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _instance = this;
        }
        
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                }

                return _instance;
            }
        }
        
        public Transform GetPlayerTransform()
        {
            return _playerTransform;
        }
    }
}