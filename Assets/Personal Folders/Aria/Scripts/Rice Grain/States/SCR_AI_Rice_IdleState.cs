using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Rice_IdleState : SCR_AI_RiceBaseStates
{
    SCR_AI_RiceGrain riceGrainScript;
    Transform enemyTransform;
    Transform playerTransform;
    float detectionRange;
    Vector3 offset;
    float sqrLen;

    Vector3 direction;
    RaycastHit hit;

    float searchRange;

    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    bool bWaveSpawn;

    public override void StartState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Rice Grain in Idle State");
        if(riceGrainScript == null )
        {
            riceGrainScript = riceGrain.GetComponent<SCR_AI_RiceGrain>();
            enemyTransform = riceGrain.transform;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (riceGrainScript.EnemyStats.bIsPartOfAWave)
            {
                bWaveSpawn = true;
            }
        }

        detectionRange = riceGrainScript.EnemyStats.DetectionRange;
        searchRange = detectionRange * 2;
        angleIncrease = (riceGrainScript.EnemyStats.EnemyFOV * 2) / rayCount;
        angle = -riceGrainScript.EnemyStats.EnemyFOV;
        meshAgent.isStopped = true;

        riceGrainScript.AnimationController.SetAnimationBool("IdleState", true);
        riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", false);
        riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", false);

        if (bWaveSpawn)
        {
            riceGrainScript.currentState = riceGrainScript.movementState;
            riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            bWaveSpawn = false;
        }
    }

    public override void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Rice Idle Update");
        offset = playerTransform.position - riceGrain.transform.position;
        sqrLen = offset.sqrMagnitude;

        if (riceGrainScript.EnemyStats.UseNewMovement)
        {
            if (sqrLen > searchRange * searchRange)
            {
                return;
            }

            direction = Quaternion.AngleAxis(angle, Vector3.up) * enemyTransform.forward;
            Vector3 origin = enemyTransform.localPosition += Vector3.up * 0.5f;
            Debug.DrawRay(enemyTransform.localPosition, direction, Color.white);

            //Raycast check from the enemy origin, in a direction of forwards + angle, with a limited range
            if (Physics.Raycast(origin, direction, out hit, detectionRange))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform == playerTransform)
                {
                    //Debug.Log("Seen Player");
                    riceGrainScript.currentState = riceGrainScript.movementState;
                    riceGrainScript.currentState.StartState(riceGrain, meshAgent);
                }
                else //Hit something that wasn't the player
                {
                    IncreaseAngle();
                }
            }
            else //Didn't hit anything
            {
                IncreaseAngle();
            }
        }
        else
        {
            if (sqrLen < detectionRange * detectionRange)
            {
                //Enter the movement state
                riceGrainScript.currentState = riceGrainScript.movementState;
                riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        
    }

    void IncreaseAngle()
    {
        if (angle == riceGrainScript.EnemyStats.EnemyFOV)
        {
            angle = -riceGrainScript.EnemyStats.EnemyFOV;
        }
        else
        {
            angle += angleIncrease;
        }
    }
}
