using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the kettle behaviour
public class SCR_ProduceSteam : MonoBehaviour
{
    //reference to the steam object
    [SerializeField] private GameObject steam;

    //time that the kettle produces steam intermittently
    [SerializeField] private float steamingTime = 1f;

    //delay length between steam emissions
    [SerializeField] private float intervalTime = 1f;

    //monitors time between kettle behaviour changes
    private float timer = 0f;

    private Transform player;

    [SerializeField] private float activationDistance = 10.0f;
    [SerializeField] private LayerMask playerLayer;
    private SCR_PlayerStats playerStats;

    private void Start()
    {
        //ensure the steam is not visible initially
        steam.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(steam.transform.position, player.transform.position) > activationDistance) { return; }

        //increase the timer by the game time
        timer += Time.deltaTime;

        //if timer is greater than interval time and the kettle is not already steaming
        if(timer >= intervalTime && !steam.activeSelf)
        {
            //produce steam
            steam.SetActive(true);
            
            //reset the timer so the lower function isn't suddenly called
            timer = 0f;
        } 
        else if(timer >= steamingTime && steam.activeSelf) //else if the timer is greater than the steaming time and the kettle is producing steam
        {
            //stop producing steam
            steam.SetActive(false);
            
            //reset the timer so the above function isn't called incorrectly
            timer = 0f;
        }
        else if(steam.activeSelf)
        {
            if (Physics.Raycast(transform.position, transform.forward, 2.3f, playerLayer, QueryTriggerInteraction.Ignore))
            {
                if (playerStats == null)
                {
                    playerStats = FindObjectOfType<SCR_PlayerStats>();
                }

                if (!playerStats.IsStunned)
                {
                    playerStats.StunPlayer(1.5f, false);
                }
            }
        }
    }
}
