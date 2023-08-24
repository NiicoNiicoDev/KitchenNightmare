using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Rice_Attack1State : SCR_AI_RiceBaseStates
{
    SCR_AI_RiceGrain riceGrainScript;
    GameObject player;
    int attackDamage;
    LayerMask playerLayerMask;
    bool bCanDealDamage;
    bool bHasDealtDamage;
    Animator animator;

    public override void StartState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Entered Rice Attack 1 State");
        riceGrainScript = riceGrain.GetComponent<SCR_AI_RiceGrain>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = riceGrainScript.AnimationController.animator;

        attackDamage = riceGrainScript.attack1Damage;
        playerLayerMask = riceGrainScript.EnemyStats.PlayerLayerMask;
        bCanDealDamage = false;
        bHasDealtDamage = false;

        meshAgent.isStopped = true;
        //meshAgent.path = null;

        riceGrainScript.AnimationController.SetAnimationBool("IdleState", false);
        riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", true);
        //riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", false);

        riceGrainScript.AudioManager.PlaySound(riceGrainScript.AudioManager.AudioClips[7], false);
    }

    public override void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        if(riceGrainScript.EnemyStats.IsStunned)
        {
            //Reset the attack and enter movement state
            bCanDealDamage = false;
            riceGrainScript.currentState = riceGrainScript.movementState;
            riceGrainScript.currentState.StartState(riceGrain, meshAgent);
        }

        bCanDealDamage = Physics.CheckSphere((riceGrain.transform.position + (Vector3.up * 0.5f)), 1.5f, playerLayerMask);

        //riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        //riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", true);

        if(bCanDealDamage && !bHasDealtDamage)
        {
            riceGrainScript.EnemyStats.PlayerStats.TakeDamage(attackDamage + riceGrainScript.EnemyStats.EnemyDamageMod);
            bHasDealtDamage = true;
        }


        if(riceGrainScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !riceGrainScript.AnimationController.animator.IsInTransition(0))
        {
            riceGrainScript.currentState = riceGrainScript.movementState;
            riceGrainScript.currentState.StartState(riceGrain, meshAgent);
        }

    }

    public override void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        
    }
}
