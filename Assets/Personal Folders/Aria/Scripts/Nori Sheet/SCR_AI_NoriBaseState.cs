using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_NoriBaseState
{
    public abstract void StartState(GameObject noriSheet, NavMeshAgent meshAgent);
    public abstract void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent);
    public abstract void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent);
}
