using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_WasabiRainState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    Transform playerTransform;
    GameObject wasabiProjectile;

    int numberOfWasabiToFire;
    float verticalOffset = 7.5f;
    float randomX;
    float randomZ;
    float enemyHealthPercentage;
    float fireDelay;
    bool bHasFinishedFiring;
    bool bHasStarted;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Wasabi Rain State");
        if(sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            wasabiProjectile = sushiRollScript.WasabiProjectilePrefab;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        enemyHealthPercentage = sushiRollScript.EnemyHealthPercentage;
        bHasFinishedFiring = false;
        bHasStarted = false;
        meshAgent.isStopped = true;
        fireDelay = sushiRollScript.FireDelay;

        if(enemyHealthPercentage >= 75f)
        {
            numberOfWasabiToFire = Random.Range(3, 6); //Upper value is exclusive, so the actual range is 3 to 5
        }
        else if (enemyHealthPercentage < 75f && enemyHealthPercentage >= 50f )
        {
            numberOfWasabiToFire = Random.Range(5, 11);
        }
        else if (enemyHealthPercentage < 50f && enemyHealthPercentage >= 25f)
        {
            numberOfWasabiToFire = Random.Range(10, 16);
        }
        else if (enemyHealthPercentage < 25f)
        {
            numberOfWasabiToFire = Random.Range(15, 21);
        }

        //Debug.Log("Firing " + numberOfWasabiToFire + " of Wasabi Peas");

        sushiRollScript.AudioManager.PlaySound(sushiRollScript.AudioManager.AudioClips[0], false);
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if (sushiRollScript.GameManager.bPlayerDead)
        {
            sushiRollScript.StopCoroutineInScript(FireWasabi());
            return;
        }

        fireDelay -= Time.deltaTime;
        if( fireDelay <= 0f )
        {
            if (!bHasStarted)
            {
                sushiRollScript.StartCoroutineInScript(FireWasabi()); //Code Adapted from RealSoftGames, 2017
                bHasStarted = true;
            }
        }

        if(bHasFinishedFiring)
        {
            sushiRollScript.StopCoroutineInScript(FireWasabi());
            //Debug.Log("Stopped Coroutine");
            sushiRollScript.currentState = sushiRollScript.movementState;
            sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }

    IEnumerator FireWasabi()
    {
        while (numberOfWasabiToFire > 0)
        {
            //Debug.Log("Coroutine Started: " + numberOfWasabiToFire);
            //Instantiate Wasabi Pea at random X and Z offset at Y units above the player
            randomX = Random.Range(-8, 9);
            randomZ = Random.Range(-8, 9);
            Vector3 spawnPos = new Vector3(sushiRollScript.transform.position.x, sushiRollScript.transform.position.y + 2f, sushiRollScript.transform.position.z);
            GameObject projectile = MonoBehaviour.Instantiate(wasabiProjectile, spawnPos, playerTransform.rotation);
            Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
            Vector3 direction = new Vector3(randomX, 4, randomZ);
            projectileRB.AddForce(direction, ForceMode.Impulse);

            //Decrease numberOfWasabiToFire
            numberOfWasabiToFire--;

            yield return new WaitForSecondsRealtime(0.1f);

        }
        //Debug.Log("Finished Coroutine");
        bHasFinishedFiring = true;
        
        /*
        //Check if number is zero
        if(numberOfWasabiToFire <= 0) //if yes then end then set bHasFinishedFiring to true
        {
            bHasFinishedFiring = true;
            yield return new WaitForSeconds(1);
        }
        else //if no then wait for 0.25 seconds
        {
            yield return new WaitForSeconds(1);
        }
        */
    }
}
