using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Flee : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        navMeshAgent.isStopped = false;

        noriSheetScript.StartRNSCoroutine(StopFleeing());
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheet.transform.rotation = Quaternion.LookRotation(noriSheet.transform.position - noriSheetScript.player.transform.position);

        Vector3 newPosition = noriSheet.transform.position + noriSheet.transform.forward * noriSheetScript.noriSpeed;

        navMeshAgent.SetDestination(newPosition);
    }

    IEnumerator StopFleeing()
    {
        yield return new WaitForSeconds(noriSheetScript.fleeTime);

        noriSheetScript.canChangeState = true;
    }
}
