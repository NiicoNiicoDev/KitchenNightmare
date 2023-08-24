using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TEMPORARY SCRIPT USED FOR TESTING. DO NOT USE IN MAIN SCENES
public class TEMP_PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 1;

    private int currentHealth = 1;

    private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;

        currentHealth = startingHealth;
    }

    public void Damage(int healthLost)
    {
        currentHealth -= healthLost;

        Debug.Log("Health remaining = " + currentHealth);

        if(currentHealth <= 0)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = spawnPoint;
        currentHealth = startingHealth;
    }
}
