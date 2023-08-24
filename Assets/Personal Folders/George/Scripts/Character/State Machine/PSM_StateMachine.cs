using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSM_StateMachine : MonoBehaviour
{
    PSM_BaseState currentState;

    private void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    private void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    public void ChangeState(PSM_BaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    protected virtual PSM_BaseState GetInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        if (GameManager.isDebugMode)
        {
            string content = currentState != null ? currentState.name : "(no current state)";
            GUILayout.Label($"<color='white'><size=40>{content}</size></color>");
        }
        
    }   
}
