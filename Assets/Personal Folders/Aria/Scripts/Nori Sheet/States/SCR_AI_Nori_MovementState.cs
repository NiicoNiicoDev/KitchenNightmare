using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class SCR_AI_Nori_MovementState : SCR_AI_NoriBaseState
{
    SCR_AI_NoriSheet noriSheetScript;
    NavMeshAgent localMeshAgent;

    float maxAttackRange;
    float sweepAttackRange;
    Vector3 offset;
    float sqrLen;
    float sqrChaseRange;

    float sweepAttackRangeSqr;
    float maxAttackRangeSqr;

    GameObject player;
    Transform playerTransform;
    Transform enemyTransform;
    bool bPreviousAttackWasSweep = false;
    bool bCowardMode = false;
    bool bCowardAttack = false;
    bool bHasDestination = false;

    Vector3 destination;
    float randomX;
    float randomZ;

    float timer;
    float cowardTimer;

    Vector3 direction;
    RaycastHit hit;

    float searchRange;

    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;
    float countdownTimer;
    bool bCountdown;

    public override void StartState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //Debug.Log("Nori Sheet Movement State");
        if(noriSheetScript == null)
        {
            noriSheetScript = noriSheet.GetComponent<SCR_AI_NoriSheet>();
            localMeshAgent = meshAgent;

            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.GetComponent<Transform>();
            enemyTransform = noriSheet.transform;

            maxAttackRange = noriSheetScript.EnemyStats.AttackRange;
            sweepAttackRange = noriSheetScript.sweepAttackRange;

            maxAttackRangeSqr = maxAttackRange * maxAttackRange;
            sweepAttackRangeSqr = sweepAttackRange * sweepAttackRange;

            sqrChaseRange = noriSheetScript.EnemyStats.ChaseRange * noriSheetScript.EnemyStats.ChaseRange;
        }
        if(Random.Range(0, 2) == 0)
        {
            bCowardMode = true;
            //Debug.Log("Nori Sheet Coward Mode");
        }
        else
        {
            bCowardMode = false;
        }
        meshAgent.isStopped = false;
        timer = noriSheetScript.EnemyStats.AttackDelay;
        cowardTimer = 10f;

        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", false);

        noriSheetScript.AudioManager.PlayRandomSound();

        angle = -noriSheetScript.EnemyStats.EnemyFOV;
        countdownTimer = 5f;
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        if(noriSheetScript.EnemyStats.IsStunned)
        {
            noriSheetScript.AnimationController.SetAnimationBool("IdleState", true);
            noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
            meshAgent.isStopped = true;
            return;
        }

        if (bCountdown)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer < 0f)
            {
                noriSheetScript.currentState = noriSheetScript.idleState;
                noriSheetScript.currentState.StartState(noriSheet, meshAgent);
            }
        }

        offset = playerTransform.position - noriSheet.transform.position;
        sqrLen = offset.sqrMagnitude;

        if(sqrLen > sqrChaseRange && noriSheetScript.EnemyStats.UseNewMovement && !bCowardMode) //Enemy will not enter idle state while in coward mode
        {
            meshAgent.isStopped = true;
            noriSheetScript.currentState = noriSheetScript.idleState;
            noriSheetScript.currentState.StartState(noriSheet, meshAgent);
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if(!bCowardMode || bCowardAttack) //if coward mode is not active or the coward enemy can attack
            {
                //Enemy can attack
                //If the player is inside the range of the sweep attack then perform a sweep attack, unless the previous attack was a sweep attack
                if (sqrLen <= sweepAttackRangeSqr && !bPreviousAttackWasSweep)
                {
                    meshAgent.isStopped = true;
                    //Player is within range and the Nori Sheet did not previously do a sweep
                    bPreviousAttackWasSweep = true;
                    //Go into sweep attack state
                    noriSheetScript.currentState = noriSheetScript.sweepAttackState;
                    noriSheetScript.currentState.StartState(noriSheet, localMeshAgent);
                    return;
                }
                else if ((sqrLen > sweepAttackRangeSqr || bPreviousAttackWasSweep) && sqrLen <= maxAttackRangeSqr) //If the player is outside the range of the sweep attack then always perform a slap attack
                {
                    meshAgent.isStopped = true;
                    //Player is outside of the range of the sweep attack but within the range of the max attack
                    //This state will run by default if the previous attack was a sweep; this stops spamming of the sweep
                    bPreviousAttackWasSweep = false;
                    //Go into slap attack state
                    noriSheetScript.currentState = noriSheetScript.slapAttackState;
                    noriSheetScript.currentState.StartState(noriSheet, localMeshAgent);
                    return;
                }
            }

        }

        if (noriSheetScript.EnemyStats.UseNewMovement)
        {
            /*if(localMeshAgent.destination == destination)
            {
                noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
                noriSheetScript.AnimationController.SetAnimationBool("MovementState", true);
            }*/

            noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
            noriSheetScript.AnimationController.SetAnimationBool("MovementState", true);

            if (bCowardMode) //Run 5u away from the player and keep running for 10 seconds, after which stand still and wait for player to approach
            {
                cowardTimer -= Time.deltaTime;
                if(cowardTimer > 0f)
                {
                    //Generates positive and negative X and Z coords that will put the enemy 5 units away from the player

                    if(sqrLen < 5 && bHasDestination && cowardTimer > 0f) //If player is closer than 5u, the enemy has a destination, but the timer is still greater than 0 then set a new destination and keep running
                    {
                        bHasDestination = false;
                    }

                    if(bHasDestination)
                    {
                        noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
                        noriSheetScript.AnimationController.SetAnimationBool("MovementState", true);
                        meshAgent.isStopped = false;
                        localMeshAgent.SetDestination(destination);
                    }
                    else
                    {
                        SetDestination();
                    }
                    
                }

                if(cowardTimer <= 0f)
                {
                    bCowardAttack = true;
                }

                if(cowardTimer < -5f)
                {
                    bCowardMode = false;
                }
            }
            
            if(sqrLen > 1f && !bCowardMode) //Approach the player
            {
                randomX = Random.Range(-2f, 2f);
                randomZ = Random.Range(-2f, 2f);
                destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);

                noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
                noriSheetScript.AnimationController.SetAnimationBool("MovementState", true);
                meshAgent.isStopped = false;
                localMeshAgent.SetDestination(destination);
            }
        }
        else
        {
            if (sqrLen > 1f)
            {
                meshAgent.isStopped = false;
                localMeshAgent.SetDestination(playerTransform.position);
            }
        }

        if (sqrLen > searchRange * searchRange)
        {
            return;
        }

        direction = Quaternion.AngleAxis(angle, Vector3.up) * enemyTransform.forward;
        Debug.DrawRay(enemyTransform.localPosition, direction, Color.white);

        //Raycast check from the enemy origin, in a direction of forwards + angle, with a limited range
        if (Physics.Raycast(enemyTransform.localPosition, direction, out hit, noriSheetScript.EnemyStats.DetectionRange))
        {
            if (hit.transform == playerTransform)
            {
                //Debug.Log("Seen Player");
                bCountdown = false;
                localMeshAgent.isStopped = false;
                countdownTimer = 5f;
            }
            else //Hit something that wasn't the player
            {
                IncreaseAngle();
                bCountdown = true;
            }
        }
        else //Didn't hit anything
        {
            IncreaseAngle();
            bCountdown = true;
        }

        if (Physics.Raycast(enemyTransform.localPosition, direction, out hit, 1f))
        {
            if (hit.transform.CompareTag("Doorway_Container") || hit.transform.name.Contains("Door"))
            {
                //Prevent enemy from moving through the doorway
                //localMeshAgent.isStopped = true;
            }
        }
    }

    void SetDestination()
    {
        if (Random.Range(0, 2) == 0)
        {
            randomX = 5;
        }
        else
        {
            randomX = -5;
        }

        if (Random.Range(0, 2) == 0)
        {
            randomZ = 5;
        }
        else
        {
            randomZ = -5;
        }

        destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
        bHasDestination = true;
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
    public override void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        
    }
}
