using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class SCR_AI_Rice_MovementState : SCR_AI_RiceBaseStates
{
    SCR_AI_RiceGrain riceGrainScript;

    float attackRange;
    Vector3 offset;
    float sqrLen;
    float sqrChaseRange;
    GameObject player;
    Transform playerTransform;
    Transform enemyTransform;
    int randomNumber;
    float timer;

    bool bHasAttacked = false;
    bool bHasDestination;
    bool bReadyToAttack;
    bool bCountdown;
    float escapeTimer;
    float randomX;
    float randomZ;
    Vector3 destination;

    Vector3 direction;
    RaycastHit hit;

    float searchRange;
    float countdownTimer;

    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    public override void StartState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Rice Grain in Movement State");
        if(riceGrainScript == null)
        {
            riceGrainScript = riceGrain.GetComponent<SCR_AI_RiceGrain>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.GetComponent<Transform>();
            enemyTransform = riceGrain.transform;
            attackRange = riceGrainScript.EnemyStats.AttackRange;
            sqrChaseRange = riceGrainScript.EnemyStats.ChaseRange * riceGrainScript.EnemyStats.ChaseRange;
            bHasAttacked = false;
        }
        
        meshAgent.isStopped = false;
        timer = riceGrainScript.EnemyStats.AttackDelay;
        escapeTimer = 2f;
        bHasDestination = false;
        bReadyToAttack = false;
        bCountdown = false;
        countdownTimer = 5f;

        searchRange = riceGrainScript.EnemyStats.DetectionRange * 2;
        angleIncrease = (riceGrainScript.EnemyStats.EnemyFOV * 2) / rayCount;
        angle = -riceGrainScript.EnemyStats.EnemyFOV;

        //meshAgent.path = null;

        riceGrainScript.AnimationController.SetAnimationBool("IdleState", false);
        riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", false);
        riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", false);

        riceGrainScript.AudioManager.PlayRandomSound();
    }

    public override void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //If the enemy is stunned, halt its movement
        if(riceGrainScript.EnemyStats.IsStunned)
        {
            meshAgent.isStopped = true;
            riceGrainScript.AnimationController.SetAnimationBool("IdleState", true);
            riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
            return;
        }

        //If player is out of LOS this countdown runs
        if(bCountdown)
        {
            countdownTimer -= Time.deltaTime;
            if(countdownTimer < 0f)
            {
                riceGrainScript.currentState = riceGrainScript.idleState;
                riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            }
        }

        offset = playerTransform.position - riceGrain.transform.position;
        sqrLen = offset.sqrMagnitude;

        //If the enemy is out of range go back into the idle state
        if(sqrLen > sqrChaseRange && riceGrainScript.EnemyStats.UseNewMovement)
        {
            meshAgent.isStopped = true;
            riceGrainScript.currentState = riceGrainScript.idleState;
            riceGrainScript.currentState.StartState(riceGrain, meshAgent);
        }

        timer -= Time.deltaTime;
        if (sqrLen < attackRange * attackRange && timer <= 0f && !bHasAttacked)
        {
            //riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
            bHasAttacked = true;
            meshAgent.isStopped = true;
            //meshAgent.path = new NavMeshPath();
            bReadyToAttack = true;
            //Debug.Log("Ready to Attack");
            //Flip a coin and enter the corrosponding attack state
            randomNumber = Random.Range(1, 3); //Random.Range uses an inclusive min and an exclusive max, so 3 will never be the result

            if (randomNumber == 1)
            {
                //Attack 1
                riceGrainScript.currentState = riceGrainScript.attack1State;
                riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            }
            else
            {
                //Attack 2
                riceGrainScript.currentState = riceGrainScript.attack2State;
                riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            }
            return;
        }

        if (riceGrainScript.EnemyStats.UseNewMovement && !bReadyToAttack)
        {
            //Same as wasabi pea, the rice grain does not know exactly where the player is, instead trying to manuvere around them
            //After the enemy has attacked it will move away from the player and try and maintain that distance before trying to flank them again
            riceGrainScript.AnimationController.SetAnimationBool("IdleState", false);
            riceGrainScript.AnimationController.SetAnimationBool("MovementState", true);

            if (bHasAttacked) //Get away from the player
            {
                if(!bHasDestination)
                {
                    GenerateDestination();
                }
                
                if(sqrLen < (2 * 2) && bHasDestination)
                {
                    GenerateDestination();
                }

                if(bHasDestination && !bReadyToAttack)
                {
                    meshAgent.isStopped = false;
                    meshAgent.SetDestination(destination);
                }

                escapeTimer -= Time.deltaTime;
                if(escapeTimer <=0)
                {
                    bHasAttacked = false;
                    escapeTimer = 2f;
                }
            }
            else //flank the player
            {
                if(!bReadyToAttack)
                {
                    randomX = Random.Range(-1f, 1f);
                    randomZ = Random.Range(-1f, 1f);
                    destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);

                    meshAgent.isStopped = false;
                    meshAgent.SetDestination(destination);
                }
            }
        }
        else
        {
            meshAgent.isStopped = false;
            meshAgent.SetDestination(playerTransform.position);
        }


        if (sqrLen > searchRange * searchRange)
        {
            return;
        }

        direction = Quaternion.AngleAxis(angle, Vector3.up) * enemyTransform.forward;
        Debug.DrawRay(enemyTransform.localPosition, direction, Color.white);

        if(!bHasAttacked)
        {
            //Raycast check from the enemy origin, in a direction of forwards + angle, with a limited range
            if (Physics.Raycast(enemyTransform.localPosition, direction, out hit, riceGrainScript.EnemyStats.DetectionRange))
            {
                if (hit.transform == playerTransform)
                {
                    //Debug.Log("Seen Player");
                    bCountdown = false;
                    meshAgent.isStopped = false;
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

            /*if (Physics.Raycast(enemyTransform.localPosition, direction, out hit, 1f))
            {
                if (hit.transform.CompareTag("Doorway_Container") || hit.transform.name.Contains("Door"))
                {
                    //Prevent enemy from moving through the doorway
                    meshAgent.isStopped = true;
                }
            }*/
        }
        
    }

    void GenerateDestination()
    {
        randomX = Random.Range(-5f, 6f);
        randomZ = Random.Range(-5f, 6f);
        destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
        bHasDestination = true;
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

    public override void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {

    }
}
