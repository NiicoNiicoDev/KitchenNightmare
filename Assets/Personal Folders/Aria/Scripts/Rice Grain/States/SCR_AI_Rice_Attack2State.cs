using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Rice_Attack2State : SCR_AI_RiceBaseStates
{
    SCR_AI_RiceGrain riceGrainScript;
    float chargeDistance;
    float chargeTimer;
    float defaultSpeed;
    float defaultAcceleration;
    Vector3 newPosition;
    GameObject player;
    LayerMask playerLayerMask;
    bool bCanDealDamage;
    bool bDealtDamage;
    bool bBegunCharge;
    Vector3 playerPosition;
    int attackDamage;
    NavMeshPath navMeshPath;
    LineRenderer line;

    Rigidbody rigidbody; //May not need

    public override void StartState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Entered Rice Attack 2 State");
        if(riceGrainScript == null)
        {
            riceGrainScript = riceGrain.GetComponent<SCR_AI_RiceGrain>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerLayerMask = riceGrainScript.EnemyStats.PlayerLayerMask;
            attackDamage = riceGrainScript.attack2Damage;
            defaultSpeed = meshAgent.speed;
            defaultAcceleration = meshAgent.acceleration;
            navMeshPath = new NavMeshPath();
            line = riceGrain.GetComponent<LineRenderer>();
        }

        line.enabled = true;
        chargeTimer = riceGrainScript.chargeTimer;
        chargeDistance = riceGrainScript.chargeDistance;
        bCanDealDamage = false;
        bDealtDamage = false;
        bBegunCharge = false;
        meshAgent.isStopped= true;
        //meshAgent.path = null;

        riceGrainScript.AnimationController.SetAnimationBool("IdleState", true);
        riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", false);
        riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", false);

        riceGrainScript.AudioManager.PlaySound(riceGrainScript.AudioManager.AudioClips[4], false);
    }

    public override void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {

        if(riceGrainScript.EnemyStats.IsStunned)
        {
            //Inturrupt and reset attack
            line.enabled = false;
            bCanDealDamage = false;
            bBegunCharge = false;
            meshAgent.speed = defaultSpeed;
            riceGrainScript.currentState = riceGrainScript.movementState;
            riceGrainScript.currentState.StartState(riceGrain, meshAgent);
        }

        chargeTimer -= Time.deltaTime;

        //Draw a line forward to show charge direction
        line.SetPosition(0, riceGrain.transform.position);
        line.SetPosition(1, riceGrain.transform.position + riceGrain.transform.forward * chargeDistance);

        
        if (chargeTimer > 1f)
        {
            playerPosition = new Vector3(player.transform.localPosition.x, 0f, player.transform.localPosition.z);
            riceGrain.transform.LookAt(playerPosition);
        }

        if(chargeTimer <= 0f && !bBegunCharge)
        {
            //Set new destination
            meshAgent.isStopped = false;
            newPosition = riceGrain.transform.localPosition;
            newPosition += riceGrain.transform.forward * chargeDistance;
            line.enabled = false;

            //Charge towards that destination if valid
            meshAgent.CalculatePath(newPosition, navMeshPath);
            meshAgent.SetPath(navMeshPath);
            meshAgent.speed = defaultSpeed * 4;
            meshAgent.acceleration = defaultAcceleration * 4f;

            /*
            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                
            }
            else if (navMeshPath.status == NavMeshPathStatus.PathPartial)
            {
                meshAgent.SetPath(navMeshPath);
                meshAgent.speed = defaultSpeed * 4;
            }
            */
            
            bBegunCharge = true;
        }

        if(bBegunCharge)
        {
            riceGrainScript.AnimationController.SetAnimationBool("IdleState", false);
            riceGrainScript.AnimationController.SetAnimationBool("MovementState", true);

            bCanDealDamage = Physics.CheckSphere((riceGrain.transform.position + (Vector3.up * 0.5f)), 1.5f, playerLayerMask);

            if(bCanDealDamage && !bDealtDamage)
            {
                meshAgent.isStopped = true;
                meshAgent.speed = defaultSpeed;
                meshAgent.acceleration = defaultAcceleration;
                bDealtDamage = true;

                riceGrainScript.EnemyStats.PlayerStats.TakeDamage(attackDamage + riceGrainScript.EnemyStats.EnemyDamageMod);

                riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
                riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", true);   
            }
            if(meshAgent.remainingDistance <= 0f)
            {
                meshAgent.isStopped = true;
                meshAgent.speed = defaultSpeed;
                meshAgent.acceleration = defaultAcceleration;

                riceGrainScript.currentState = riceGrainScript.movementState;
                riceGrainScript.currentState.StartState(riceGrain, meshAgent);
            }

            if(bDealtDamage)
            {
                if (riceGrainScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !riceGrainScript.AnimationController.animator.IsInTransition(0))
                {
                    riceGrainScript.currentState = riceGrainScript.movementState;
                    riceGrainScript.currentState.StartState(riceGrain, meshAgent);
                }
            }
        }
        
    }

    public override void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {

    }
}
