using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_MovementState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    NavMeshAgent localMeshAgent;
    bool bCanUseStrike;

    float timer;
    float delay;
    float sqrAttackRange;
    float sqrStrikeRange;
    float sqrChaseRange;

    Transform playerTransform;
    Vector3 offset;
    float sqrLen;

    Vector3 destination;
    float randomX;
    float randomZ;

    float searchRange;
    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    Vector3 direction;
    Vector3 origin;
    RaycastHit hit;

    bool bCountdown;
    float countdownTimer;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        //Debug.Log("Salmon Chunk Movement State");
        if(salmonChunkScript == null)
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            localMeshAgent = meshAgent;
            sqrAttackRange = salmonChunkScript.EnemyStats.AttackRange * salmonChunkScript.EnemyStats.AttackRange;
            sqrStrikeRange = salmonChunkScript.StrikeRange * salmonChunkScript.StrikeRange;
            sqrChaseRange = salmonChunkScript.EnemyStats.ChaseRange * salmonChunkScript.EnemyStats.ChaseRange;

            searchRange = salmonChunkScript.EnemyStats.DetectionRange * 2;
            angleIncrease = (salmonChunkScript.EnemyStats.EnemyFOV * 2) / rayCount;
        }

        
        if (salmonChunkScript.bEnraged)
        {
            bCanUseStrike = true;
        }

        meshAgent.isStopped = true;
        bCountdown = false;
        timer = salmonChunkScript.EnemyStats.AttackDelay;
        delay = 2f;

        salmonChunkScript.AnimationController.SetAnimationBool("IdleState", true);
        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SpinAttackState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("StrikeAttackState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SlapAttackState", false);

        salmonChunkScript.AudioManager.PlayRandomSound();

        countdownTimer = 5f;
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        if (salmonChunkScript.EnemyStats.IsStunned)
        {
            meshAgent.isStopped = true;
            return;
        }

        if(bCountdown)
        {
            countdownTimer -= Time.deltaTime;
            if(countdownTimer < 0)
            {
                salmonChunkScript.currentState = salmonChunkScript.idleState;
                salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
            }
        }

        if(localMeshAgent.isStopped)
        {
            salmonChunkScript.AnimationController.SetAnimationBool("IdleState", true);
            salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        }
        else
        {
            salmonChunkScript.AnimationController.SetAnimationBool("IdleState", false);
            salmonChunkScript.AnimationController.SetAnimationBool("MovementState", true);
        }

        offset = playerTransform.position - salmonChunk.transform.position;
        sqrLen = offset.sqrMagnitude;

        if(sqrLen > sqrChaseRange && salmonChunkScript.EnemyStats.UseNewMovement)
        {
            meshAgent.isStopped = true;
            salmonChunkScript.currentState = salmonChunkScript.idleState;
            salmonChunkScript.currentState.StartState(salmonChunk, localMeshAgent);
        }

        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        if(timer <= 0f)
        {
            //Perform an attack
            int random = Random.Range(1, 4);
            //bCanUseStrike = true; //Remove when finished with testing

            if(sqrLen <= sqrStrikeRange && bCanUseStrike && random == 3)
            {
                //Strike Attack
                localMeshAgent.isStopped = true;
                salmonChunkScript.currentState = salmonChunkScript.strikeAttackState;
                salmonChunkScript.currentState.StartState(salmonChunk, localMeshAgent);
                return;
            }
            else if(sqrLen <= sqrAttackRange && random == 2)
            {
                //Enter the Spin State
                localMeshAgent.isStopped = true;
                salmonChunkScript.currentState = salmonChunkScript.spinAttackState;
                salmonChunkScript.currentState.StartState(salmonChunk, localMeshAgent);
                return;
            }
            else if (sqrLen <= sqrAttackRange && random == 1)
            {
                //Slap Attack
                localMeshAgent.isStopped = true;
                salmonChunkScript.currentState = salmonChunkScript.slapAttackState;
                salmonChunkScript.currentState.StartState(salmonChunk, localMeshAgent);
                return;
            }
            //return;
        }

        if (salmonChunkScript.EnemyStats.UseNewMovement)
        {
            //If the player is over 5 units away or is above 50% health enemy waits here for 3 seconds before approaching
            //If player is closer or below half health then it will not wait and head straight towards the player
            
            /*randomX = Random.Range(-2f, 2f);
            randomZ = Random.Range(-2f, 2f);
            destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
            localMeshAgent.SetDestination(destination);*/
            
            //meshAgent.isStopped = false;

            if (sqrLen > 5 || salmonChunkScript.EnemyStats.PlayerStats.currentHealth > 50)
            {
                delay -= Time.deltaTime;
                if(delay < 0)
                {
                    SetDestination();
                    
                }
            }
            else
            {
                //Debug.Log("Player too far");
                SetDestination();
            }
        }
        else
        {
            if (sqrLen > 3f)
            {
                meshAgent.isStopped = false;
                localMeshAgent.SetDestination(playerTransform.position);
            }
        }

        if (sqrLen > searchRange * searchRange)
        {
            return;
        }

        direction = Quaternion.AngleAxis(angle, Vector3.up) * salmonChunk.transform.forward;
        origin = salmonChunk.transform.localPosition + (0.25f * salmonChunk.transform.up);
        Debug.DrawRay(origin, direction, Color.white);

        if (Physics.Raycast(origin, direction, out hit, salmonChunkScript.EnemyStats.DetectionRange))
        {
            if (hit.transform == playerTransform)
            {
                bCountdown = false;
                countdownTimer = 5f;
                localMeshAgent.isStopped = false;
            }
            else
            {
                IncreaseAngle();
                bCountdown = true;
            }
        }
        else
        {
            IncreaseAngle();
            bCountdown = true;
        }

        if (Physics.Raycast(origin, direction, out hit, 1f))
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
        localMeshAgent.isStopped = false;
        randomX = Random.Range(-1f, 1f);
        randomZ = Random.Range(-1f, 1f);
        destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
        localMeshAgent.SetDestination(destination);
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
