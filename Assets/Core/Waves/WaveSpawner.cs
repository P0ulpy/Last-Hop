using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyAndSpawnParameters
    {
        public GameObject enemy;
        public int numberOfEnemies;
    }

    [System.Serializable]
    public class Wave
    {
        public EnemyAndSpawnParameters[] enemiesAndSpawnParameters;
        public Vector2 minMaxTimeBetweenSpawns;
        public float timeBeforeStartingWave;
        public float orthoSizeToZoomOutAtStart;
    }

    [System.Serializable]
    public class SpawnPointsByCategory
    {
        public BaseEnemy.EnemyTypes[] enemiesSpawningIntoIt;
        public Transform[] allTransforms;
    }

    [Header("General")]
    [SerializeField] private Wave[] _waves;
    [SerializeField] private CameraMovement _cameraMovement;

    [Header("Spawn points")]
    [SerializeField] private SpawnPointsByCategory _groundSpawnPoints;
    [SerializeField] private SpawnPointsByCategory _windowsSpawnPoints;
    [SerializeField] private SpawnPointsByCategory _skySpawnPoints;

    [Header("Reset to 0 when shipping")]
    [SerializeField] private int _waveIndexToStart = 0;

    private Wave _currentWave;
    private int _currentWaveIndex;
    private bool _finishedSpawning;

    private void Start()
    {
        _currentWaveIndex = _waveIndexToStart;

        StartFirstWave();
    }

    public void StartFirstWave()
    {
        StartCoroutine(StartNextWave(_currentWaveIndex, true));
    }

    private IEnumerator StartNextWave(int index, bool ignoreWaiting = false)
    {
        //On attend timeBetweenWaves secondes, puis on lance une vague

        _currentWave = _waves[index]; //Le vague courante est celle désignée par currentWaveIndex
        _cameraMovement.ZoomCameraOut(_currentWave.orthoSizeToZoomOutAtStart);

        if(!ignoreWaiting)
        {
            yield return new WaitForSeconds(_currentWave.timeBeforeStartingWave);
        }
        else 
        {
            yield return null; 
        }

        StartCoroutine(SpawnCurrentWave());
    }

    private IEnumerator SpawnCurrentWave()
    {
        //Récupérer tous les ennmis dans une liste.
        List<GameObject> enemies = GetAllEnemiesOfCurrentWave();
        int totalOfEnemiesOfCurrentWave = enemies.Count;

        for (int i = 0; i < totalOfEnemiesOfCurrentWave ; i++)
        {
            //if(player == null)
            //{//S'il est mort, on arrête de spawn de monstres
            //    yield break;
            //}

            int randomEnemyIndex = UnityEngine.Random.Range(0, enemies.Count);
            GameObject randomEnemy = enemies[randomEnemyIndex];
            enemies.RemoveAt(randomEnemyIndex);

            Transform randomSpotToSpawn = GetRandomSpawnPoint(randomEnemy.GetComponent<BaseEnemy>());
            Instantiate(randomEnemy, randomSpotToSpawn.position, randomSpotToSpawn.rotation);

            //Détection de la fin de la vague
            if(i == totalOfEnemiesOfCurrentWave - 1)
            {
                _finishedSpawning = true;
            }
            else
            {
                _finishedSpawning = false;
            }

            //Attendre le temps qu'il faut entre chaque spawn de monstre
            yield return new WaitForSeconds(UnityEngine.Random.Range(_currentWave.minMaxTimeBetweenSpawns.x, _currentWave.minMaxTimeBetweenSpawns.y));
        }
    }

    private void Update()
    {
        if (_finishedSpawning == true && GameObject.FindGameObjectsWithTag("Enemy").Length == 0) // Very ugly, what can I do
        {
            _finishedSpawning = false; //Si on a finit la vague, on setup la suivante

            if(_currentWaveIndex + 1 < _waves.Length) //S'il y a encore une vague/des vagues
            {
                PrepareNextWave();
                StartCoroutine(StartNextWave(_currentWaveIndex));
            }
            else//S'il n'y en a plus
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Debug.Log("gg la street t'as gagné");
            }
        }
    }

    private void PrepareNextWave()
    {
        _currentWaveIndex++;
        Debug.Log("La vague " + (_currentWaveIndex + 1) + " va commencer...");
    }

    private Transform GetRandomSpawnPoint(BaseEnemy enemyToSpawn)
    {
        switch (enemyToSpawn.EnemyType)
        {
            case BaseEnemy.EnemyTypes.LesFDPQuiCourts:
                return _groundSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _groundSpawnPoints.allTransforms.Length)];

            case BaseEnemy.EnemyTypes.LesFDPQuiTirent:
                return _windowsSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _windowsSpawnPoints.allTransforms.Length)];

            case BaseEnemy.EnemyTypes.LesDronesDeFDP:
                return _skySpawnPoints.allTransforms[UnityEngine.Random.Range(0, _skySpawnPoints.allTransforms.Length)];

            default:
                Debug.LogWarning("WaveSpawner: Wtf un autre type d'ennemi ?");
                return _groundSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _groundSpawnPoints.allTransforms.Length)];
        }
    }

    private List<GameObject> GetAllEnemiesOfCurrentWave()
    {
        List<GameObject> enemies = new List<GameObject>();

        foreach (var enemiesAndSpawnParameters in _currentWave.enemiesAndSpawnParameters)
        {
            for (int i = 0; i < enemiesAndSpawnParameters.numberOfEnemies; i++)
            {
                enemies.Add(enemiesAndSpawnParameters.enemy);
            }
        }

        return enemies;
    }
}
