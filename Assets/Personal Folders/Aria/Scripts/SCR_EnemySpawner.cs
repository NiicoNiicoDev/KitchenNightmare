using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class SCR_EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnParticle;

    [Header("Fixed Spawning variables")]
    [Tooltip("Use Scriptable Objects for wave spawning, this will be disabled if the array of Waves is empty")]
    [SerializeField] bool bUseFixedSpawning = false;

    [Tooltip("Wave Scriptable Object that will be spawned when using Fixed Wave Spawning")]
    [SerializeField] public SO_FixedEnemyWave enemyWaveSO;

    [Header("Random Spawning Variables")]
    [Tooltip("Enemy Prefabs, this can be adjusted to limit which enemies are spawned, this is used to randomly generate a wave")]
    [SerializeField] GameObject[] enemyPrefabs = new GameObject[4];
    [SerializeField] int maxNumberOfWaves;
    [SerializeField] int maxNumberOfSpawns;

    [Header("Spawn Area Size")]
    [Tooltip("These values should be half the Size of the Box Collider")]
    [SerializeField] public float areaSizeX = 1f;
    [SerializeField] public float areaSizeZ = 1f;

    [Header("Debug - Please Do Not Edit")]
    [Tooltip("Limits the number of Nori Sheet and Salmon Chunk enemies to 1 per wave")]
    [SerializeField] bool bLimitStrongEnemies = false;
    [SerializeField] int numberOfWaves;
    [SerializeField] Wave enemyWave;

    int numberOfSpawns;
    int currentWave = 1;

    public bool bEncounterStarted { get; private set; } = false;
    bool bWaveOngoing = false;
    bool bWaveReady = false;
    bool bGeneratingWave = false;
    bool bHasSpawnedSalmon = false;
    bool bHasSpawnedNori = false;
    bool bHasSpawnedRottenSalmon = false;
    bool bHasSpawnedRottenNori = false;

    public bool bEnemyTypeDefeated = false;

    Collider trigger;
    Transform playerTransform;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if(enemyWaveSO == null || enemyWaveSO.enemyWaves.Count == 0)
        {
            bUseFixedSpawning = false;
        }
        trigger = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //I removed this because I figured we didn't need it past the alpha demo - Joe
        /*if(Input.GetKeyDown(KeyCode.F6))
        {
            if(!bUseFixedSpawning)
            {
                numberOfWaves = Random.Range(1, maxNumberOfWaves + 1);
            }
            else
            {
                numberOfWaves = enemyWaveSO.enemyWaves.Count;
            }
            currentWave = 0;
            bWaveOngoing = false;
            bEncounterStarted = true;
        }*/

        if (numberOfSpawns == 0 && bWaveOngoing)
        {
            bWaveOngoing = false;
            numberOfWaves--;
            //Debug.Log("Wave Over");
        }

        if (bEncounterStarted)
        {
            //The encounter has started, loop through the waves and call the SpawnWave coroutine for each one
            if (!bWaveOngoing)
            {
                //Start the next wave if there is one, otherwise end the encounter and disable the spawner
                if(numberOfWaves == 0)
                {
                    //Encounter is over
                    trigger.enabled = false;
                    bEncounterStarted = false;
                    GameManager.gameManager.DecrementSpawnersInLevel();
                    //Debug.Log("Encounter is over");
                }
                else
                {
                    bGeneratingWave = false;
                    bWaveReady = false;
                    NextWave();
                }
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            if (bEncounterStarted) { return; }
            playerTransform = other.transform;
            //Start the wave spawning
            currentWave = 0;
            if(!bUseFixedSpawning)
            {
                numberOfWaves = Random.Range(1, maxNumberOfWaves + 1);
            }
            else
            {
                numberOfWaves = enemyWaveSO.enemyWaves.Count;
            }

            bEncounterStarted = true;
        }
    }

    public void EnemyKilled()
    {
        numberOfSpawns--;
    }

    private void NextWave()
    {
        currentWave++;
        //Next wave
        if (!bUseFixedSpawning)
        {
            //Random spawning
            //Generate a new wave using the Coroutine
            if (!bGeneratingWave)
            {
                StartCoroutine(GenerateRandomWave());
                bGeneratingWave = true;
            }

            if (bWaveReady)
            {
                //Save the number of enemies in the current wave
                //Start the coroutine to spawn the enemies
                numberOfSpawns = enemyWave.enemiesInWave.Length;
                StartCoroutine(SpawnWave(enemyWave));
                bWaveOngoing = true;
            }
        }
        else
        {
            //Fixed spawning
            //Saves the current wave to a local variable and stores the number of enemies in the wave
            enemyWave = enemyWaveSO.enemyWaves[currentWave - 1];
            numberOfSpawns = enemyWave.enemiesInWave.Length;

            //Starts the coroutine before removing the wave at index 0, this should cause all the other items in the list to shift down 1 allowing the code to repeat without needing a loop
            StartCoroutine(SpawnWave(enemyWave));
            bWaveOngoing = true;
        }
    }

    //Generate a random wave using the custom Wave struct
    IEnumerator GenerateRandomWave()
    {
        int numOfEnemies = Random.Range(5, maxNumberOfSpawns + 1);
        int randomEnemyIndex;
        GameObject[] waveEnemies = new GameObject[numOfEnemies];

        for(int i = 0; i < numOfEnemies; i++)
        {
            randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            if(bLimitStrongEnemies)
            {
                if (bHasSpawnedNori && enemyPrefabs[randomEnemyIndex].transform.name.Contains("AI_NoriSheet"))
                {
                    waveEnemies[i] = enemyPrefabs[0];
                    continue;
                }
                if (bHasSpawnedSalmon && enemyPrefabs[randomEnemyIndex].transform.name.Contains("AI_SalmonChunk"))
                {
                    waveEnemies[i] = enemyPrefabs[1];
                    continue;
                }

                if((bHasSpawnedRottenNori && enemyPrefabs[randomEnemyIndex].transform.name.Contains("NoriSheet_Idle")) || (bHasSpawnedRottenSalmon && enemyPrefabs[randomEnemyIndex].transform.name.Contains("SalmonChunk_Idle")))
                {
                    waveEnemies[i] = enemyPrefabs[4];
                    continue;
                }
            }

            waveEnemies[i] = enemyPrefabs[randomEnemyIndex];

            if(bLimitStrongEnemies)
            {
                if (enemyPrefabs[randomEnemyIndex].transform.name.Contains("AI_NoriSheet"))
                {
                    bHasSpawnedNori = true;
                }
                if (enemyPrefabs[randomEnemyIndex].transform.name.Contains("AI_SalmonChunk"))
                {
                    bHasSpawnedSalmon = true;
                }
                if (enemyPrefabs[randomEnemyIndex].transform.name.Contains("NoriSheet_Idle"))
                {
                    bHasSpawnedRottenNori = true;
                }
                if (enemyPrefabs[randomEnemyIndex].transform.name.Contains("SalmonChunk_Idle"))
                {
                    bHasSpawnedRottenSalmon = true;
                }
            }
        }

        enemyWave = new Wave(waveEnemies);
        bWaveReady = true;
        //Debug.Log("Generated Wave");
        yield return null;
    }

    //Spawn the enemies that are a part of the wave
    IEnumerator SpawnWave(Wave enemyWave)
    {
        //Spawn position needs to be a random X and Z
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        spawnPosition.x = transform.position.x;
        spawnPosition.z = transform.position.z;
        GameObject enemy;

        int totalNumOfSpawns = numberOfSpawns;

        for (int i = 0; i < totalNumOfSpawns; i++)
        {
            /*spawnPosition.x = Random.Range(-areaSizeX, areaSizeX);
            spawnPosition.z = Random.Range(-areaSizeZ, areaSizeZ);*/
            //enemy = Instantiate(enemyWave.enemiesInWave[i], gameObject.transform.localPosition + spawnPosition, gameObject.transform.rotation);
            /*enemy = Instantiate(enemyWave.enemiesInWave[i], gameObject.transform.position + spawnPosition, gameObject.transform.rotation);*/
            enemy = Instantiate(enemyWave.enemiesInWave[i], spawnPosition, gameObject.transform.rotation);
            enemy.transform.LookAt(playerTransform);
            enemy.GetComponent<SCR_EnemyStats>().SpawnInWave(this);
            spawnedEnemies.Add(enemy);
            if (!enemy.GetComponent<NavMeshAgent>().isOnNavMesh)
            {
                Destroy(enemy);
                i--;
            }
            else
            {
                /*if(spawnParticle != null)
                {
                    *//*GameObject particles = Instantiate(spawnParticle, gameObject.transform.position + spawnPosition, gameObject.transform.rotation);*//*
                    GameObject particles = Instantiate(spawnParticle, enemy.transform.position, gameObject.transform.rotation);
                    particles.transform.parent = enemy.transform;
                    particles.GetComponent<ParticleSystem>().Play();
                    Destroy(particles, 1f);
                }*/
                enemy.GetComponent<SCR_EnemyDeathFade>().StartGrowIn();
            }
            yield return new WaitForSeconds(2f);
        }

        //Debug.Log("Finished Spawning");
        yield return null;
    }

    public void ResetSpawner()
    {
        bEncounterStarted = false;
        bWaveOngoing = false;
        bWaveReady = false;
        bGeneratingWave = false;
        bHasSpawnedSalmon = false;
        bHasSpawnedNori = false;
        trigger.enabled = true;
        currentWave = 0;


        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
            }
        }
        spawnedEnemies.Clear();
    }
}
