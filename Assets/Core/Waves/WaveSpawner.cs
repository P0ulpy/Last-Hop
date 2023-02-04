using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPointsByEnemyTypes
    {
        public BaseEnemy.EnemyTypes[] enemiesSpawningIntoIt;
        public Transform[] allTransforms;
    }

    [Header("General")]
    [SerializeField] private Wave[] _waves;
    [SerializeField] private CameraMovement _cameraMovement;

    [Header("Spawn points")]
    [SerializeField] private SpawnPointsByEnemyTypes _groundSpawnPoints;
    [SerializeField] private SpawnPointsByEnemyTypes _windowsSpawnPoints;
    [SerializeField] private SpawnPointsByEnemyTypes _skySpawnPoints;

    [Header("Reset to 0 when shipping")]
    [SerializeField] private int _waveIndexToStart = 0;

    private Wave _currentWave;
    private int _currentWaveIndex;
    private bool _finishedSpawning;

    private List<Vector3> _allWindowsPointsOccupied;

    private void Awake()
    {
        _currentWaveIndex = _waveIndexToStart;

        _allWindowsPointsOccupied = new List<Vector3>();
    }

    private void Start()
    {
        foreach (var wave in _waves)
        {
            wave.Init();
        }

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
        List<BaseEnemy> enemies = GetAllEnemiesOfCurrentWave();
        int totalOfEnemiesOfCurrentWave = enemies.Count;

        for (int i = 0; i < totalOfEnemiesOfCurrentWave ; i++)
        {
            bool hasSpawned = false;

            while (!hasSpawned)
            {
                if (CanEnemySpawn())
                {
                    // Get random enemy from array
                    int randomEnemyIndex = UnityEngine.Random.Range(0, enemies.Count);
                    BaseEnemy randomEnemy = enemies[randomEnemyIndex];

                    // Get random spot to spawn enemy
                    Vector3 randomSpotToSpawn;
                    bool hasFoundSucceed = GetRandomSpawnPoint(randomEnemy.EnemyType, out randomSpotToSpawn);

                    if (hasFoundSucceed)
                    {
                        // Spawn enemy at random spot
                        enemies.RemoveAt(randomEnemyIndex);
                        BaseEnemy spawnedEnemy = Instantiate(randomEnemy, randomSpotToSpawn, Quaternion.identity);
                        
                        spawnedEnemy.SetTarget(GameManager.Instance.GetPlayerTransform());

                        OnWaveAddEnemy(randomSpotToSpawn);

                        // On player death
                        spawnedEnemy.OnDeathCallback += () =>
                        {
                            OnWaveRemoveEnemy(randomSpotToSpawn);
                        };

                        //Détection de la fin de la vague
                        _finishedSpawning = i == totalOfEnemiesOfCurrentWave - 1;
                        hasSpawned = true;
                    }
                }

                //Attendre le temps qu'il faut entre chaque spawn de monstre
                yield return new WaitForSeconds(UnityEngine.Random.Range(_currentWave.minMaxTimeBetweenSpawns.x, _currentWave.minMaxTimeBetweenSpawns.y));
            }
        }
    }

    private void Update()
    {
        if (_finishedSpawning && _currentWave.CurrentDeadEnemies >= _currentWave.NumEnemies)
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

    /*
     Returns true if we found a spot to spawn your enemy

     If there is already too much enemies, don't spawn.
     If an enemy will spawn, try to find a transform until a transform is non occupied.
        If all types are gone, all spots are occupied, we can't spawn anything.
     */
    private bool GetRandomSpawnPoint(BaseEnemy.EnemyTypes enemyTypeToSpawn, out Vector3 randomSpawnPoint)
    {
        randomSpawnPoint = new Vector3();

        // ----
        switch (enemyTypeToSpawn)
        {
            case BaseEnemy.EnemyTypes.LesFDPQuiCourts:
                randomSpawnPoint = _groundSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _groundSpawnPoints.allTransforms.Length)].position;
                return true;

            case BaseEnemy.EnemyTypes.LesDronesDeFDP:
                randomSpawnPoint = _skySpawnPoints.allTransforms[UnityEngine.Random.Range(0, _skySpawnPoints.allTransforms.Length)].position;
                return true;

            case BaseEnemy.EnemyTypes.LesFDPQuiTirent:
                break;

            default:
                randomSpawnPoint = _groundSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _groundSpawnPoints.allTransforms.Length)].position;
                return true;
        }


        List<Vector3> tempPositionOccupied = new List<Vector3>();

        for (int i = 0; i < _windowsSpawnPoints.allTransforms.Length; i++)
        {
            Vector3 newWindowPosition = _windowsSpawnPoints.allTransforms[UnityEngine.Random.Range(0, _windowsSpawnPoints.allTransforms.Length)].position;
            
            if(!tempPositionOccupied     .Contains(newWindowPosition) && 
               !_allWindowsPointsOccupied.Contains(newWindowPosition))
            {
                randomSpawnPoint = newWindowPosition;
                return true;
            }
            else
            {
                tempPositionOccupied.Add(newWindowPosition);
            }
        }
        
        Debug.Log("GetRandomSpawnPoint failed ! No window available, apparently...");
        return false;
    }

    private List<BaseEnemy> GetAllEnemiesOfCurrentWave()
    {
        List<BaseEnemy> enemies = new List<BaseEnemy>();

        foreach (var enemiesAndSpawnParameters in _currentWave.enemiesAndSpawnParameters)
        {
            for (int i = 0; i < enemiesAndSpawnParameters.numberOfEnemies; i++)
            {
                enemies.Add(enemiesAndSpawnParameters.enemy);
            }
        }

        return enemies;
    }

    private void OnWaveAddEnemy(Vector3 spawnSpot)
    {
        _allWindowsPointsOccupied.Add(spawnSpot);
        _currentWave.CurrentNumSpawnedEnnemies++;
    }

    private void OnWaveRemoveEnemy(Vector3 spawnSpot)
    {
        _allWindowsPointsOccupied.Remove(spawnSpot);
        _currentWave.CurrentNumSpawnedEnnemies--;
        _currentWave.CurrentDeadEnemies++;
    }

    public bool CanEnemySpawn()
    {
        bool canSpawn = _currentWave.CurrentNumSpawnedEnnemies < _currentWave.maxEnemiesSpawned; 

        if(!canSpawn)
        {
            Debug.Log("Enemy can't spawn, due to max enemy into wave");
        }

        return canSpawn;
    }
}
