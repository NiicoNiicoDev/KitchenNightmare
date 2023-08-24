using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_HazardSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private GameObject hazardPrefab;

    [Tooltip("If this value is less than the positions array it will randomly choose which positions to spawn in")]
    [SerializeField] private int numberOfHazards = 1;

    private void Start()
    {
        if (positions.Length == numberOfHazards)
        {
            SpawnAll();
        }
        else if (positions.Length < numberOfHazards)
        {
            numberOfHazards = positions.Length;
            RandomizeSpawn();
        }
        else
        {
            RandomizeSpawn();
        }
    }

    private void RandomizeSpawn()
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i < numberOfHazards; i++)
        {
            int rand = Random.Range(0, positions.Length);
            while (numbers.Contains(rand))
            {
                rand = Random.Range(0, positions.Length);
            }
            numbers.Add(rand);
            Instantiate(hazardPrefab, positions[rand].position, positions[rand].rotation);
        }
    }

    private void SpawnAll()
    {
        for (int i = 0; i < numberOfHazards; i++)
        {
            Instantiate(hazardPrefab, positions[i].position, positions[i].rotation);
        }
    }
}
