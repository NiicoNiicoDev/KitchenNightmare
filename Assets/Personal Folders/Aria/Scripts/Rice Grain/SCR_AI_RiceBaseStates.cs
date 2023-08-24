using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_RiceBaseStates
{
    public abstract void StartState(GameObject riceGrain, NavMeshAgent meshAgent);
    public abstract void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent);
    public abstract void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent);
}
