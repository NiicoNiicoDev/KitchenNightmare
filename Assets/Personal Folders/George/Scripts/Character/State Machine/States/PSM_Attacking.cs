using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSM_Attacking : PSM_BaseState
{
    private PSM_MovementStateMachine _sm;
    private PSM_InputHandler input;

    public PSM_Attacking(PSM_MovementStateMachine stateMachine) : base("Attacking", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        input = _sm.GetComponent<PSM_InputHandler>();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }
}
