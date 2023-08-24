using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This state performs is the idle state, the enemy waits here until the player has entered its line of sight at which point it moves to the movement state.
//This state is only used with the new movement system
public class SCR_AI_Wasabi_IdleState : SCR_AI_WasabiBaseStates
{
    SCR_AI_WasabiPea wasabiPeaScript;
    Transform playerTransform;
    Transform enemyTransform;

    Vector3 direction;
    Vector3 origin;
    RaycastHit hit;

    float searchRange;

    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    Vector3 offset;
    float sqrLen;
    bool bWaveSpawn;

    public override void StartState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        if(wasabiPeaScript == null)
        {
            wasabiPeaScript = wasabiPea.GetComponent<SCR_AI_WasabiPea>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            enemyTransform = wasabiPea.transform;
            angleIncrease = (wasabiPeaScript.EnemyStats.EnemyFOV * 2) / rayCount;
            searchRange = wasabiPeaScript.EnemyStats.DetectionRange * 2;

            if (wasabiPeaScript.EnemyStats.bIsPartOfAWave)
            {
                bWaveSpawn = true;
            }
            //Debug.Log("Angle Increase: " + angleIncrease);
        }

        wasabiPeaScript.AnimationController.SetAnimationBool("IdleState", true);
        wasabiPeaScript.AnimationController.SetAnimationBool("MovementState", false);

        if (!wasabiPeaScript.EnemyStats.UseNewMovement)
        {
            wasabiPeaScript.currentState = wasabiPeaScript.movementState;
            wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
        }

        if (bWaveSpawn || wasabiPeaScript.EnemyStats.bSpawnedByBoss)
        {
            //wasabiPeaScript.EnemyStats.bSpawnedByBoss = false;
            bWaveSpawn = false;
            wasabiPeaScript.currentState = wasabiPeaScript.movementState;
            wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
        }

        angle = -wasabiPeaScript.EnemyStats.EnemyFOV;

        wasabiPeaScript.AudioManager.PlayRandomSound();
    }

    public override void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        offset = playerTransform.localPosition - enemyTransform.localPosition;
        sqrLen = offset.sqrMagnitude;
        if(sqrLen > searchRange * searchRange)
        {
            return;
        }

        //start from negative enemy FOV (angle = -enemyFOV)
        //Raycast, then angle = angle + angleIncrease
        //Check again until angle == emnemyFOV at which point repeat
        //Debug.Log("Angle: " + angle);

        direction = Quaternion.AngleAxis(angle, Vector3.up) * enemyTransform.forward;
        origin = enemyTransform.localPosition + (0.25f * enemyTransform.up);
        Debug.DrawRay(origin, direction, Color.white);

        //Raycast check from the enemy origin, in a direction of forwards + angle, with a limited range
        if(Physics.Raycast(origin, direction, out hit, wasabiPeaScript.EnemyStats.DetectionRange))
        {
            if(hit.transform == playerTransform)
            {
                //Debug.Log("Seen Player");
                wasabiPeaScript.currentState = wasabiPeaScript.movementState;
                wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
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

    public override void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        
    }

    void IncreaseAngle()
    {
        if (angle == wasabiPeaScript.EnemyStats.EnemyFOV)
        {
            angle = -wasabiPeaScript.EnemyStats.EnemyFOV;
        }
        else
        {
            angle += angleIncrease;
        }
    }
}
