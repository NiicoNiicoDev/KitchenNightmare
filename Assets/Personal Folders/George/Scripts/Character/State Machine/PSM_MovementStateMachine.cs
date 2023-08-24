using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PSM_MovementStateMachine : PSM_StateMachine
{
    public enum controlType
    {
        Mouse,
        Keyboard,
        Gamepad
    }

    public controlType inputMethod = controlType.Keyboard;

    public NavMeshAgent agent;

    public PSM_Idle idleState;
    public PSM_Moving movingState;

    public bool canMove;
    public bool weaponDisabledMovement { get; private set; } = false;

    public Vector3 targetDirection;

    [SerializeField] private ParticleSystem runEffect;

    private void Awake()
    {
        idleState = new PSM_Idle(this);
        movingState = new PSM_Moving(this);
        agent = GetComponent<NavMeshAgent>();
    }

    protected override PSM_BaseState GetInitialState()
    {
        return movingState;
    }

    private void FixedUpdate()
    {
        if (SCR_AudioManager.instance == null)
            return;
        
        if (movingState.IsMoving && !runEffect.isPlaying)
        {
            runEffect.Play();
            SCR_AudioManager.instance.SetPlayerWalking(true);
        }
        else if (!movingState.IsMoving && runEffect.isPlaying)
        {
            runEffect.Stop();
            SCR_AudioManager.instance.SetPlayerWalking(false);
        }
    }

    public void SetWeaponDisabledMovement(bool value)
    {
        canMove = !value;
        weaponDisabledMovement = value;
    }
}
