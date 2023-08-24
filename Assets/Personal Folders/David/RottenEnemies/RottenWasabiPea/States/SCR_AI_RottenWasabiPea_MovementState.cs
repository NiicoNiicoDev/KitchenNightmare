using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RottenWasabiPea_MovementState : SCR_AI_RottenWasabiPea_BaseStates
{
    private SCR_AI_RottenWasabiPea rWPScript;

    private SCR_StunEnemy stunEnemy;

    public override void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Movement State");
        rWPScript = rottenWasabiPea.GetComponent<SCR_AI_RottenWasabiPea>();
        stunEnemy = rottenWasabiPea.GetComponent<SCR_StunEnemy>();
    }

    public override void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        if (stunEnemy.IsStunned)
        {
            navMeshAgent.isStopped = true;
            rWPScript.wasabiAnimator.SetBool("isMoving", false);
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(rWPScript.player.transform.position);

        rWPScript.wasabiAnimator.SetBool("isMoving", true);
    }
}
