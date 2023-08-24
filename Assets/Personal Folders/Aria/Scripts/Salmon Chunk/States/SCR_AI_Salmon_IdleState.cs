using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_IdleState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    Transform playerTransform;
    float sqrDetectionRange;
    Vector3 offset;
    float sqrLen;

    float searchRange;
    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    Vector3 direction;
    Vector3 origin;
    RaycastHit hit;

    bool bWaveSpawn;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        Debug.Log("Salmon Chunk Idle State");
        if(salmonChunkScript == null)
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            sqrDetectionRange = salmonChunkScript.EnemyStats.DetectionRange * salmonChunkScript.EnemyStats.DetectionRange;
            searchRange = salmonChunkScript.EnemyStats.DetectionRange * 2;
            angleIncrease = (salmonChunkScript.EnemyStats.EnemyFOV * 2) / rayCount;

            if (salmonChunkScript.EnemyStats.bIsPartOfAWave)
            {
                bWaveSpawn = true;
            }
        }
        angle = -salmonChunkScript.EnemyStats.EnemyFOV;
        salmonChunkScript.AnimationController.SetAnimationBool("IdleState", true);
        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);

        if(bWaveSpawn)
        {
            salmonChunkScript.currentState = salmonChunkScript.movementState;
            salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
            bWaveSpawn = false;
        }

        meshAgent.isStopped = true;
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        offset = playerTransform.position - salmonChunk.transform.position;
        sqrLen = offset.sqrMagnitude;
        
        

        if(salmonChunkScript.EnemyStats.UseNewMovement)
        {
            if(sqrLen > searchRange * searchRange)
            {
                return;
            }

            direction = Quaternion.AngleAxis(angle, Vector3.up) * salmonChunk.transform.forward;
            origin = salmonChunk.transform.localPosition + (0.25f * salmonChunk.transform.up);
            Debug.DrawRay(origin, direction, Color.white);

            if (Physics.Raycast(origin, direction, out hit, salmonChunkScript.EnemyStats.DetectionRange))
            {
                if(hit.transform == playerTransform)
                {
                    salmonChunkScript.currentState = salmonChunkScript.movementState;
                    salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
                }
                else
                {
                    IncreaseAngle();
                }
            }
            else
            {
                IncreaseAngle();
            }
        }
        else
        {
            if (sqrLen < sqrDetectionRange)
            {
                salmonChunkScript.currentState = salmonChunkScript.movementState;
                salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
            }
        }
        
    }
    void IncreaseAngle()
    {
        if (angle == salmonChunkScript.EnemyStats.EnemyFOV)
        {
            angle = -salmonChunkScript.EnemyStats.EnemyFOV;
        }
        else
        {
            angle += angleIncrease;
        }
    }

    public override void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        
    }
}
