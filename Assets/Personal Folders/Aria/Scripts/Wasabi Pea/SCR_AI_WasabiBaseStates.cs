using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_WasabiBaseStates
{
    public abstract void StartState(GameObject wasabiPea, NavMeshAgent meshAgent);
    public abstract void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent);
    public abstract void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent);
}
