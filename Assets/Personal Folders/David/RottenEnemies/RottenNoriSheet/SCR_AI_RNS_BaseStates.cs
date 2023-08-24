using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_RNS_BaseStates
{
    public abstract void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent);

    public abstract void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent);
}
