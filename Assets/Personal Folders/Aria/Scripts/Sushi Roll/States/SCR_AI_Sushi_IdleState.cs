using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_IdleState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    SCR_EnemyCounter enemyCounter;

    Transform playerTransform;
    float sqrActivationRange;
    Vector3 offset;
    float sqrLen;
    float wakeupTimer;

    bool bPlayerInRange;

    NavMeshAgent localMeshAgent;
    Rigidbody rb;
    SkinnedMeshRenderer meshRenderer;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Idle State");

        if(sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            rb = sushiRoll.GetComponent<Rigidbody>();
            meshRenderer = sushiRoll.GetComponentInChildren<SkinnedMeshRenderer>();
            enemyCounter = GameObject.FindGameObjectWithTag("GameController").GetComponent<SCR_EnemyCounter>();

            localMeshAgent = meshAgent;
            sqrActivationRange = sushiRollScript.ActivationRange * sushiRollScript.ActivationRange;
        }

        wakeupTimer = sushiRollScript.WakeupTimer;
        bPlayerInRange = false;

        sushiRollScript.AnimationController.SetAnimationBool("MovementState", false);
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if(enemyCounter.bWasabiDefeated && enemyCounter.bRiceDefeated && enemyCounter.bNoriDefeated && enemyCounter.bSalmonDefeated)
        {
            if (!sushiRollScript.bIsReady)
            {
                sushiRollScript.bIsReady = true;
                GameManager.gameManager.PlayBossIntroMusic();
            }
            //Debug.Log("Boss Ready: " + sushiRollScript.bIsReady);
            sushiRollScript.AnimationController.SetAnimationBool("IdleState", true);
            //Debug.Log("Ready to Wake Up");
        }

        //Sushi can only start wakeup once at least one of each enemy has been beaten
        if(sushiRollScript.bIsReady && !sushiRollScript.GameManager.bPlayerDead)
        {
            offset = playerTransform.position - sushiRoll.transform.position;
            sqrLen = offset.sqrMagnitude;
            if (sqrLen < sqrActivationRange)
            {
                //start wakeup
                //Debug.Log("Waking Up");
                bPlayerInRange = true;
                meshRenderer.enabled = true;
                rb.useGravity = true;
            }

            if (bPlayerInRange)
            {
                wakeupTimer -= Time.deltaTime;
            }

            if (wakeupTimer <= 0f)
            {
                //Debug.Log("Fight Time");
                //sushiRollScript.HealthBar.SetActive(true);
                localMeshAgent.enabled = true;
                sushiRollScript.bIsAwake = true;
                sushiRollScript.currentState = sushiRollScript.movementState;
                sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        
    }
}
