using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_MovementState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;

    int randomAttackModifier;
    int randomAttackNumber;

    bool isEnraged;
    bool isAttacking;
    bool previouslyAttacked = false;
    float attackDelayTimer;

    Transform playerTransform;
    Vector3 offset;
    float sqrLen;
    float sqrAttackRange;

    NavMeshAgent localMeshAgent;

    Vector3 destination;
    float bossDifference;
    float playerDifference;
    float randomX;
    float randomZ;
    float timer;
    bool bPlayerOutOfRange;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Movement State");
        if(sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            localMeshAgent = meshAgent;
            sqrAttackRange = sushiRollScript.MaxAttackRange * sushiRollScript.MaxAttackRange;
        }

        if(sushiRollScript.bEnraged)
        {
            attackDelayTimer = sushiRollScript.AttackTimer / 2;
            randomAttackModifier = 1;
        }
        else
        {
            attackDelayTimer = sushiRollScript.AttackTimer;
            randomAttackModifier = 0;
        }

        localMeshAgent.isStopped = false;
        bPlayerOutOfRange = false;
        isAttacking = false;
        sushiRollScript.AnimationController.SetAnimationBool("IdleState", false);
        sushiRollScript.AnimationController.SetAnimationBool("MovementState", true);
        timer = 1f;
        sushiRollScript.AudioManager.PlayRandomSound();
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        bossDifference = sushiRoll.transform.localPosition.magnitude - sushiRollScript.StartingPosition.magnitude;
        playerDifference = playerTransform.position.magnitude - sushiRollScript.WorldStartingPos.magnitude;
        
        if(playerDifference <= 6f)
        {
            bPlayerOutOfRange = false;
        }
        else
        {
            bPlayerOutOfRange = true;
        }
        
        if(bossDifference > 5f)
        {
            //Debug.Log("Boss too far from centre of room!");
            //Boss stops, waits one second, then turns and heads back to the centre of the room
            //If the player comes within 5u of the centre of the room then it will stop moving back to the centre and instead start attacking
            meshAgent.isStopped = true;
            //Debug.Log("Returning to room");
            timer -= Time.deltaTime;
            if(timer <= 0f && bPlayerOutOfRange)
            {
                meshAgent.SetDestination(sushiRollScript.WorldStartingPos);
                meshAgent.isStopped = false;
                if(sushiRoll.transform.localPosition.magnitude == (sushiRollScript.StartingPosition.magnitude + 0.2f))
                {
                    sushiRollScript.currentState = sushiRollScript.idleState;
                    sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
                }
            }
        }

        if (bPlayerOutOfRange) return;

        offset = playerTransform.position - sushiRoll.transform.position;
        sqrLen = offset.sqrMagnitude;

        attackDelayTimer -= Time.deltaTime;

        if(sqrLen < sqrAttackRange && attackDelayTimer < 0f)
        {
            //Player is within attack range
            //Boss can attack the player
            //This is random, but the random attack modifier is added to the random value to favour stronger attacks
            randomAttackNumber = (Random.Range(1, 7) + randomAttackModifier); //If 1 or 6 is rolled then there is no attack, if 6 is the total value (6 on the random plus 1 from the random attack mod) then it's a 50/50 chance between Attack 3 and 4
            //randomAttackNumber = 1;
            switch(randomAttackNumber)
            {
                case 1:
                    //No attack just loop and try again. This will never be called if enraged
                    break;
                case 2:
                    //Attack 1 - Wasabi Rain
                    sushiRollScript.currentState = sushiRollScript.wasabiRainState;
                    sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    isAttacking = true;
                    previouslyAttacked = true;
                    break;
                case 3:
                    //Attack 2 - Rice Missiles
                    sushiRollScript.currentState = sushiRollScript.riceMissilesState;
                    sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    isAttacking = true;
                    previouslyAttacked = true;
                    break;
                case 4:
                    //Attack 3 - Salmon Stomp
                    sushiRollScript.currentState = sushiRollScript.salmonStompState;
                    sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    isAttacking = true;
                    previouslyAttacked = true;
                    break;
                case 5:
                    //Attack 4 - Nori Spin
                    if (playerDifference > 1.5f) break; //Boss is too far from the centre
                    sushiRollScript.currentState = sushiRollScript.noriSpinState;
                    sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    isAttacking = true;
                    previouslyAttacked = true;
                    break;
                case 6:
                    //No attack, just loop
                    break;
                case 7:
                    //Coin flip for Attack 3 or 4. This can only be called if enraged
                    randomAttackNumber = Random.Range(1, 3);
                    if(randomAttackNumber == 1)
                    {
                        //Attack 3
                        sushiRollScript.currentState = sushiRollScript.salmonStompState;
                        sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    }
                    else
                    {
                        //Attack 4
                        sushiRollScript.currentState = sushiRollScript.noriSpinState;
                        sushiRollScript.currentState.StartState(sushiRoll, localMeshAgent);
                    }
                    isAttacking = true;
                    previouslyAttacked = true;
                    break;
                default:
                    //Something's gone wrong, just loop
                    Debug.LogWarning("Error in randomAttackNumber Switch Statement");
                    break;
            }
            if(isAttacking)
            {
                return;
            }
        }

        if(sushiRollScript.EnemyStats.UseNewMovement)
        {
            //If it did not previously attack the boss will head towards the player
            //Otherwise the boss will wait for the attackDelayTimer to expire before moving
            if(previouslyAttacked)
            {
                if(attackDelayTimer <= 0f)
                {
                    previouslyAttacked = false;
                }
            }
            else
            {
                randomX = Random.Range(-2f, 2f);
                randomZ = Random.Range(-2f, 2f);
                destination = new Vector3(playerTransform.localPosition.x + randomX, playerTransform.localPosition.y, playerTransform.localPosition.z + randomZ);
                localMeshAgent.SetDestination(destination);
            }
        }
        else
        {
            if (sqrLen > 2f * 2f)
            {
                localMeshAgent.isStopped = false;
                sushiRollScript.AnimationController.SetAnimationBool("IdleState", false);
                sushiRollScript.AnimationController.SetAnimationBool("MovementState", true);
                localMeshAgent.SetDestination(playerTransform.position);
            }
            else
            {
                localMeshAgent.isStopped = true;
                /*sushiRollScript.AnimationController.SetAnimationBool("IdleState", true);
                sushiRollScript.AnimationController.SetAnimationBool("MovementState", false);*/
            }
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }
}
