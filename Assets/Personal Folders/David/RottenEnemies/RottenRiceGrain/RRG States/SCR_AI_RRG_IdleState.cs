using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RRG_IdleState : SCR_AI_RRG_BaseStates
{
    private SCR_AI_RottenRiceGrain riceGrainScript;

    public override void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Idle State");
        riceGrainScript = rottenRiceGrain.GetComponent<SCR_AI_RottenRiceGrain>();

        navMeshAgent.isStopped = true;

        riceGrainScript.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

        riceGrainScript.riceGrainAnimator.SetBool("IsMoving", false);
    }

    public override void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        //rottenRiceGrain.transform.LookAt(riceGrainScript.player.transform);
    }
}
