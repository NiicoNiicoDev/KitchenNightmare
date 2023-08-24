using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deprecated_TestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject testEnemyPrefab;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Timer Point Stuff")]
    [SerializeField] private int timeToAdd = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Instantiate(testEnemyPrefab, transform.position + spawnOffset, Quaternion.identity);
            //SCR_ScoreTracker.instance.AddTimeToScore(timeToAdd);
            GameManager.gameManager.LevelEnded();
        }
    }
}
