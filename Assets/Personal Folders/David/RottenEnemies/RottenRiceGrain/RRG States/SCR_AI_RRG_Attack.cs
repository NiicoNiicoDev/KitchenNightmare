using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RRG_Attack : SCR_AI_RRG_BaseStates
{
    private SCR_AI_RottenRiceGrain riceGrainScript;

    private float defaultSpeed;

    private float timer = 0;

    public override void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Attack State");
        riceGrainScript = rottenRiceGrain.GetComponent<SCR_AI_RottenRiceGrain>();

        defaultSpeed = navMeshAgent.speed;

        navMeshAgent.speed = riceGrainScript.swoopSpeed;

        timer = 0f;

        riceGrainScript.riceGrainAnimator.SetTrigger("Attacking");

        AnimatorClipInfo[] clipInfo = riceGrainScript.riceGrainAnimator.GetCurrentAnimatorClipInfo(0);
        riceGrainScript.attackLength = clipInfo[0].clip.length;
    }

    public override void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        navMeshAgent.isStopped = false;

        navMeshAgent.SetDestination(riceGrainScript.player.transform.position);
        timer += Time.deltaTime;

        if (timer > riceGrainScript.attackLength)
        {
            riceGrainScript.timeSinceLastAttack = 0f;
        }
    }
}
