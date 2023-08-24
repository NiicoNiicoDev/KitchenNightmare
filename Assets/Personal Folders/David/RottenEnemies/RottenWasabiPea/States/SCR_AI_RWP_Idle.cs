using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RWP_Idle : SCR_AI_RottenWasabiPea_BaseStates
{
    private SCR_AI_RottenWasabiPea rottenWasabi;

    public override void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Idle State");
        if (!rottenWasabi)
        {
            rottenWasabi = rottenWasabiPea.GetComponent<SCR_AI_RottenWasabiPea>();
        }

        navMeshAgent.isStopped = true;

        rottenWasabi.wasabiAnimator.SetBool("isMoving", false);
    }

    public override void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        rottenWasabiPea.transform.LookAt(rottenWasabi.player.transform); //this prevents the AI from getting stuck in walls.
    }
}
