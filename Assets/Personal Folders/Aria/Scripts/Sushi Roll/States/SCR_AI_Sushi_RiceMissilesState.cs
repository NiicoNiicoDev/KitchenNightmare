using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_RiceMissilesState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    GameObject defualtRiceMissile;
    GameObject currentRiceMissile;
    Transform sushiRollTransform;

    int numOfRiceMissiles;
    float fireDelay;
    float randomX;
    float randomZ;
    bool bHasStarted;
    bool bHasFinished;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Rice Missiles State");
        if (sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            defualtRiceMissile = sushiRollScript.RiceMissilePrefab;
            sushiRollTransform = sushiRoll.transform;
        }

        numOfRiceMissiles = sushiRollScript.NumOfRiceMissiles;
        fireDelay = sushiRollScript.FireDelay;
        bHasStarted = false;
        bHasFinished = false;
        meshAgent.isStopped = true;

        sushiRollScript.AudioManager.PlaySound(sushiRollScript.AudioManager.AudioClips[3], false);
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if(sushiRollScript.GameManager.bPlayerDead)
        {
            sushiRollScript.StopCoroutineInScript(FireRice());
            return;
        }

        fireDelay -= Time.deltaTime;
        if (fireDelay <= 0 && !bHasStarted)
        {
            sushiRollScript.StartCoroutineInScript(FireRice());
            bHasStarted = true;
        }

        if(bHasFinished)
        {
            sushiRollScript.StopCoroutineInScript(FireRice());
            sushiRollScript.currentState = sushiRollScript.movementState;
            sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }

    IEnumerator FireRice()
    {
        while(numOfRiceMissiles > 0)
        {
            randomX = Random.Range(-1f, 1f);
            randomZ = Random.Range(-1f, 1f);
            int randomSeeking = Random.Range(1, 3);
            Vector3 spawnPos = new Vector3(sushiRollTransform.position.x + randomX, 2f, sushiRollTransform.position.z + randomZ);
            currentRiceMissile = MonoBehaviour.Instantiate(defualtRiceMissile, spawnPos, Quaternion.identity);
            //currentRiceMissile.GetComponent<SCR_RiceMissile>().Fire(false);

            //randomSeeking = 2;

            if (randomSeeking == 1)
            {
                currentRiceMissile.GetComponent<SCR_RiceMissile>().Fire(false);
            }
            else
            {
                currentRiceMissile.GetComponent<SCR_RiceMissile>().Fire(true);
            }

            numOfRiceMissiles--;

            yield return new WaitForSecondsRealtime(0.1f);
        }
        bHasFinished = true;
    }
}
