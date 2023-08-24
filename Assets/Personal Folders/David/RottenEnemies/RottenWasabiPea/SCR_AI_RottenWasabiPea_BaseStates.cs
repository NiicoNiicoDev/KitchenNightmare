using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_RottenWasabiPea_BaseStates
{
    public abstract void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent);

    public abstract void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent);
}
