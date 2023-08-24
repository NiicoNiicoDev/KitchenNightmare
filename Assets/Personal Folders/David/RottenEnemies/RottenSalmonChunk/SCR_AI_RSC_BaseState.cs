using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public abstract class SCR_AI_RSC_BaseState
{
    public abstract void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent);

    public abstract void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent);

    public abstract void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent);
}
