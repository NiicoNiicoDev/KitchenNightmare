using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SCR_AI_SushiBaseStates
{
    public abstract void StartState(GameObject sushiRoll, NavMeshAgent meshAgent);

    public abstract void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent);

    public abstract void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent);
}
