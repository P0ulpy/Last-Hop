using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    [System.Serializable]
    public class EnemyAndSpawnParameters
    {
        public BaseEnemy enemy;
        public int numberOfEnemies;
    }

    public EnemyAndSpawnParameters[] enemiesAndSpawnParameters;
    public Vector2 minMaxTimeBetweenSpawns;
    public int maxEnemiesSpawned;
    public float timeBeforeStartingWave;
    public float orthoSizeToZoomOutAtStart;

    [HideInInspector] public int CurrentNumSpawnedEnnemies;
    [HideInInspector] public int CurrentDeadEnemies;

    private int _numEnemies;
    public int NumEnemies => _numEnemies;

    public void Init()
    {
        _numEnemies = 0;

        foreach (var item in enemiesAndSpawnParameters)
        {
            _numEnemies += item.numberOfEnemies;
        }
    }
}
