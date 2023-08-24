using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is depreciated
public class SCR_EnemyHealth : MonoBehaviour
{
    [SerializeField] int _enemyHealth = 10;

    [HideInInspector] public SCR_ScoringSystem scoringSystem;

    [HideInInspector] public bool justDamaged = false;

    public int CurrentHealth
    {
        get
        {
            return _enemyHealth;
        }
        private set
        {
            _enemyHealth = value;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        justDamaged = true;

        if(CurrentHealth <= 0)
        {
            if (scoringSystem)
            {
                //scoringSystem.EnemyKilled(this);
            }
        }
    }
}
