using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_RRG_BaseStates
{
    public abstract void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent);

    public abstract void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent);
}
