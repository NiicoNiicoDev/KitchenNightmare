using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SCR_CooldownSpawner : MonoBehaviour
{
    private Vector3[] spawnPositions;
    [SerializeField] private GameObject cooldownPickupPrefab;

    [SerializeField] private int numberOfSpawns = 5;
    [SerializeField] private int[] indexSequence;
    private int pointer = 0;
    public void FindSpawnPositions()
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Cooldown_Pickup_Position");
        spawnPositions = new Vector3[spawns.Length];
        indexSequence = new int[spawns.Length];

        for (int i = 0; i < spawns.Length; i++)
        {
            spawnPositions[i] = spawns[i].transform.position;
            indexSequence[i] = i;
        }

        ShuffleIndexSequence();
        SpawnCooldownPickup();
    }

    private void SpawnCooldownPickup()
    {
        for (int i = 0; i < numberOfSpawns; i++, pointer++)
        {
            Vector3 newPosition = spawnPositions[indexSequence[pointer]];
            GameObject cooldownPickupObj = Instantiate(cooldownPickupPrefab, newPosition, Quaternion.identity);

            cooldownPickupObj.GetComponent<SCR_CooldownPickup>().spawner = this;
        }
    }

    private void ShuffleIndexSequence()
    {
        System.Random random = new System.Random();
        indexSequence = indexSequence.OrderBy(x => random.Next()).ToArray();
    }

    public void SetNewPosition(GameObject currentPickup)
    {
        Vector3 newPosition = spawnPositions[indexSequence[pointer]];
        currentPickup.transform.position = newPosition;

        if (pointer == spawnPositions.Length - 1)
        {
            pointer = 0;
        }
        else
        {
            pointer++;
        }
    }
}
