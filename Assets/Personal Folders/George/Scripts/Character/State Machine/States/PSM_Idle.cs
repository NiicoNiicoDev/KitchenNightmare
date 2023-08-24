using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSM_Idle : PSM_BaseState
{
    private PSM_MovementStateMachine _sm;
   public PSM_Idle (PSM_MovementStateMachine stateMachine) : base("Idle", stateMachine) 
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.GetComponent<Animator>().SetBool("isMoving", false);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_sm.inputMethod == PSM_MovementStateMachine.controlType.Keyboard)
        {
            if (_sm.GetComponent<PSM_InputHandler>().CheckKBInput != Vector2.zero)
            {
                _sm.ChangeState(_sm.movingState);
            }
        }

        if (_sm.inputMethod == PSM_MovementStateMachine.controlType.Mouse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _sm.ChangeState(_sm.movingState);

            }
        }
    }
}
