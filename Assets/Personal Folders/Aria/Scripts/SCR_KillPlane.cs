using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_KillPlane : MonoBehaviour
{
    SCR_EnemyStats enemyStats;
    SCR_PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Object is a player
            playerStats.TakeDamage(playerStats.currentHealth);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            //Object is either a boss or an enemy
            enemyStats = other.GetComponent<SCR_EnemyStats>();
            enemyStats.TakeDamage(enemyStats.CurrentHealth);
        }
    }
}
