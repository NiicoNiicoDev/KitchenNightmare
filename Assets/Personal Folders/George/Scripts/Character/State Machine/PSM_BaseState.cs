using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSM_BaseState
{
    public string name;
    protected PSM_StateMachine stateMachine;

    public PSM_BaseState(string name, PSM_StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }

}
