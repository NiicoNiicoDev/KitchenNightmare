using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TEMPORARY SCRIPT USED FOR TESTING. DO NOT USE IN MAIN SCENES
public class TEMP_EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health = 1;

    [HideInInspector] public SCR_ScoringSystem scoringSystem;

    public void Damage(float damage)
    {
        /*
        health -= damage;
        Debug.Log("remaining health = " + health);

        if(health <= 0)
        {
            if (scoringSystem)
            {
                scoringSystem.EnemyKilled(this);
            }

            Destroy(gameObject);
        }
        */
    }
}
