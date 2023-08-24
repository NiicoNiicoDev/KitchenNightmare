using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Wasabi_MovementState : SCR_AI_WasabiBaseStates
{
    SCR_AI_WasabiPea wasabiPeaScript;

    NavMeshAgent localMeshAgent; //stores the meshAgent that's passed over from the WasabiPea script for quick access in the update & fixed update function
    float detectionRange;
    float attackRange;
    GameObject player;
    Transform playerTransform;
    Transform enemyTransform;
    bool bHasSeenPlayer;

    Vector3 direction;
    Vector3 origin;
    RaycastHit hit;
    bool bCountdown;

    float searchRange;

    int rayCount = 10;
    float angle = 0f;
    float angleIncrease;

    float timer;
    float lossTimer;
    float chaseRangeSqr;

    Vector3 destination;
    float randomX;
    float randomZ;

    public override void StartState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Debug.Log("Wasabi Movement State Start");
        if(wasabiPeaScript == null) //Checks if the script has been assigned, if not then this is the first time this Start function has been run
        {
            wasabiPeaScript = wasabiPea.GetComponent<SCR_AI_WasabiPea>();

            localMeshAgent = meshAgent;
            detectionRange = wasabiPeaScript.EnemyStats.DetectionRange;
            attackRange = wasabiPeaScript.EnemyStats.AttackRange;
            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.GetComponent<Transform>();
            chaseRangeSqr = wasabiPeaScript.EnemyStats.ChaseRange * wasabiPeaScript.EnemyStats.ChaseRange;
            bHasSeenPlayer = false;

            enemyTransform = wasabiPea.transform;
            angleIncrease = (wasabiPeaScript.EnemyStats.EnemyFOV * 2) / rayCount;
            searchRange = wasabiPeaScript.EnemyStats.DetectionRange * 2;
        }
        else //This is here for when the enemy returns to here from another state
        {
            localMeshAgent.isStopped = false;
            bHasSeenPlayer = true; //bHasSeenPlayer needs to be set to true so that the enemy does not loose track of the player
        }

        wasabiPeaScript.AnimationController.SetAnimationBool("IdleState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("AttackState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("MovementState", true);

        angle = -wasabiPeaScript.EnemyStats.EnemyFOV;

        lossTimer = 5.0f;
        timer = wasabiPeaScript.EnemyStats.AttackDelay; //Sets the timer to the Attack Delay timer, this prevents the enemy from attacking immediately as this state is entered
        wasabiPeaScript.AudioManager.PlayRandomSound();
    }

    public override void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        if(wasabiPeaScript.EnemyStats.IsStunned)
        {
            localMeshAgent.isStopped = true;
            return;
        }

        //Debug.Log("Wasabi Movement State Update");
        if(timer > 0) //Only decrement the timer if it is above 0
        {
            timer -= Time.deltaTime;
        }
        
        //Wasabi should wait until the player is within range and then begin tracking the player
        //Code adapted from Unity Technologies, 2021(b)
        Vector3 offset = playerTransform.position - wasabiPea.transform.position; //calculates the difference between the player's position and the enemy's
        float sqrLen = offset.sqrMagnitude; //uses the square magnitude function instead of the magnitude, as this is slightly more performant

        if(sqrLen > chaseRangeSqr && wasabiPeaScript.EnemyStats.UseNewMovement)
        {
            //Go back to idle state; player is out of range
            localMeshAgent.isStopped = true;
            bHasSeenPlayer = false;
            wasabiPeaScript.currentState = wasabiPeaScript.idleState;
            wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
        }
        
        if (sqrLen < attackRange * attackRange && timer <= 0f) //If the player is in range of an attack and the timer has hit zero then stop the enemy and enter the Attack State
        {
            //Switch to attack state
            localMeshAgent.isStopped = true;
            wasabiPea.transform.LookAt(playerTransform);
            wasabiPeaScript.currentState = wasabiPeaScript.attackState;
            wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
        }

        if(bCountdown)
        {
            lossTimer -= Time.deltaTime;
            if(lossTimer < 0f)
            {
                localMeshAgent.isStopped = true;
                bHasSeenPlayer = false;
                wasabiPeaScript.currentState = wasabiPeaScript.idleState;
                wasabiPeaScript.currentState.StartState(wasabiPea, meshAgent);
            }
        }

        if (wasabiPeaScript.EnemyStats.UseNewMovement)
        {
            //Enemy should not know with 100% certainty where the player is, only a general idea of where the player is
            //Each frame enemy should pick a space within 2u x 2u of the players location and set its destination for that point
            randomX = Random.Range(-2f, 2f);
            randomZ = Random.Range(-2f, 2f);
            destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
            localMeshAgent.SetDestination(destination);
        }
        else //Old Movement Code
        {
            if (sqrLen < detectionRange * detectionRange) //Checks if the result of the above calculation is less than the detection range squared
            {
                //The player is close
                localMeshAgent.isStopped = false;
                bHasSeenPlayer = true; //Sets the bHasSeenPlayer to true, this means that the enemy has seen the player before and so if the player exits the detection range then the enemy will continue following
                //wasabiPea.transform.LookAt(playerTransform.position); //Looks at the player, this is important for the attack state
                localMeshAgent.SetDestination(playerTransform.position); //Sets the destination for the nav mesh agent to the players position
            }
            else if ((sqrLen > detectionRange * detectionRange) && (bHasSeenPlayer))
            {
                //Keep following player
                localMeshAgent.isStopped = false;
                //wasabiPea.transform.LookAt(playerTransform.position);
                localMeshAgent.SetDestination(playerTransform.position);
            }
            else if (wasabiPeaScript.EnemyStats.bIsPartOfAWave)
            {
                //If the enemy is a part of a wave spawning system then immediately head towards the player
                localMeshAgent.isStopped = false;
                bHasSeenPlayer = true;
                //wasabiPea.transform.LookAt(playerTransform.position);
                localMeshAgent.SetDestination(playerTransform.position);
            }
            //End of adapted code
        }

        if (sqrLen > searchRange * searchRange)
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
        if (Physics.Raycast(origin, direction, out hit, wasabiPeaScript.EnemyStats.DetectionRange))
        {
            if (hit.transform == playerTransform)
            {
                bCountdown = false;
                lossTimer = 5.0f;
                localMeshAgent.isStopped = false;
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

        if (Physics.Raycast(origin, direction, out hit, 1f))
        {
            if (hit.transform.CompareTag("Doorway_Container") || hit.transform.name.Contains("Door"))
            {
                //Prevent enemy from moving through the doorway
                //localMeshAgent.isStopped = true;
            }
        }
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

    public override void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        
    }
}
