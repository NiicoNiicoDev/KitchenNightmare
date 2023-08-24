using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_SalmonBaseStates
{
    public abstract void StartState(GameObject salmonChunk, NavMeshAgent meshAgent);
    public abstract void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent);
    public abstract void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent);
}
