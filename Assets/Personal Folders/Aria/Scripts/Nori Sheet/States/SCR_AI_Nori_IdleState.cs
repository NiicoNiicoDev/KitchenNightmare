using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Nori_IdleState : SCR_AI_NoriBaseState
{
    SCR_AI_NoriSheet noriSheetScript;
    GameObject player;
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

    public override void StartState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //Debug.Log("Nori Sheet Idle State");
        if(noriSheetScript == null)
        {
            noriSheetScript = noriSheet.GetComponent<SCR_AI_NoriSheet>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.GetComponent<Transform>();

            detectionRange = noriSheetScript.EnemyStats.DetectionRange;
            searchRange = detectionRange * 2;
            angleIncrease = (noriSheetScript.EnemyStats.EnemyFOV * 2) / rayCount;
            if(noriSheetScript.EnemyStats.bIsPartOfAWave)
            {
                bWaveSpawn = true;
            }
        }

        angle = -noriSheetScript.EnemyStats.EnemyFOV;

        noriSheetScript.AnimationController.SetAnimationBool("IdleState", true);
        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", false);

        if (bWaveSpawn)
        {
            noriSheetScript.currentState = noriSheetScript.movementState;
            noriSheetScript.currentState.StartState(noriSheet, meshAgent);
            bWaveSpawn = false;
        }
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {

        offset = playerTransform.position - noriSheet.transform.position;
        sqrLen = offset.sqrMagnitude;

        

        if(noriSheetScript.EnemyStats.UseNewMovement)
        {
            if(sqrLen > searchRange * searchRange)
            {
                //Debug.Log("Player Out Of Range");
                return;
            }

            direction = Quaternion.AngleAxis(angle, Vector3.up) * noriSheet.transform.forward;
            Debug.DrawRay(noriSheet.transform.localPosition + (0.25f * Vector3.up), direction, Color.white);

            //Raycast check from the enemy origin, in a direction of forwards + angle, with a limited range
            if (Physics.Raycast(noriSheet.transform.localPosition + (0.25f * Vector3.up), direction, out hit, noriSheetScript.EnemyStats.DetectionRange))
            {
                if (hit.transform == playerTransform)
                {
                    //Debug.Log("Seen Player");
                    noriSheetScript.currentState = noriSheetScript.movementState;
                    noriSheetScript.currentState.StartState(noriSheet, meshAgent);
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
                noriSheetScript.currentState = noriSheetScript.movementState;
                noriSheetScript.currentState.StartState(noriSheet, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        
    }

    void IncreaseAngle()
    {
        if (angle == noriSheetScript.EnemyStats.EnemyFOV)
        {
            angle = -noriSheetScript.EnemyStats.EnemyFOV;
        }
        else
        {
            angle += angleIncrease;
        }
    }
}
