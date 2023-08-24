using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PSM_Moving : PSM_BaseState
{
    private PSM_MovementStateMachine _sm;
    private PSM_InputHandler input;

    private Coroutine mouseMovementRoutine;

    public PSM_Moving (PSM_MovementStateMachine stateMachine) : base("Moving", stateMachine) 
    {
        _sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        input = _sm.GetComponent<PSM_InputHandler>();
        _sm.GetComponent<Animator>().SetBool("isMoving", true);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (_sm.canMove)
        {
            switch (_sm.inputMethod)
            {
                case PSM_MovementStateMachine.controlType.Mouse:
                    MouseMovement();
                    break;

                case PSM_MovementStateMachine.controlType.Keyboard:
                    KeyboardMovement();
                    break;

                case PSM_MovementStateMachine.controlType.Gamepad:
                    GamepadMovement();
                    break;
            }
        }

        if (!_sm.agent.hasPath && _sm.agent.pathEndPosition == _sm.agent.transform.position)
        {
            _sm.ChangeState(_sm.idleState);
        }

    }

    #region Mouse Movement
    void MouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            _sm.agent.destination = input.MousePositionToWorldPosition();
        }
    }
    #endregion

    #region Keyboard Movement

    Vector3 relativeUp = new Vector3(1, 0, -1);
    Vector3 relativeDown = new Vector3(-1, 0, 1);
    Vector3 relativeRight = new Vector3(-1, 0, -1);
    Vector3 relativeLeft = new Vector3(1, 0, 1);

    //Vector3 relativeUpRight = new Vector3(0.707107f, 0, 0.707107f);
    //Vector3 relativeDownRight = new Vector3(0.707107f, 0, -0.707107f);
    //Vector3 relativeUpLeft = new Vector3(-0.707107f, 0, -0.707107f);
    //Vector3 relativeDownLeft = new Vector3(-0.707107f, 0, 0.707107f);

    void KeyboardMovement()
    {
        /*if (input.targetDirection != Vector3.zero)
        {
            _sm.agent.destination = _sm.transform.position + input.targetDirection;
        }
        else
        {
            _sm.agent.destination = _sm.transform.position;
        }*/

        Vector3 currentPos = _sm.gameObject.transform.position;
        Vector3 targetPos = Vector3.zero;

        Vector3 targetDir = input.targetDirection;
        targetDir.Normalize();

        targetPos = Vector3.Lerp(currentPos, currentPos + targetDir, _sm.agent.speed * Time.deltaTime);

        _sm.gameObject.transform.position = targetPos;
        _sm.gameObject.transform.rotation = Quaternion.Slerp(_sm.gameObject.transform.rotation, Quaternion.LookRotation(input.targetDirection), 5f * Time.deltaTime);
    }

    Vector3 GetDirection(Vector2 inputDirection)
    {
        Vector3 targetDirection = Vector3.zero;

        switch (inputDirection)
        {
            //Cardinal directions
            case Vector2 v when v.Equals(Vector2.up):
                targetDirection = Vector3.up + Vector3.right;
                break;

            case Vector2 v when v.Equals(Vector2.down):

                break;
                

            case Vector2 v when v.Equals(Vector2.right):

                break;

            case Vector2 v when v.Equals(Vector2.left):

                break;

            //Non-Cardinal directions
            case Vector2 v when v.Equals(Vector2.up + Vector2.right):

                break;

            case Vector2 v when v.Equals(Vector2.up + Vector2.left):

                break;

            case Vector2 v when v.Equals(Vector2.down + Vector2.right):

                break;

            case Vector2 v when v.Equals(Vector2.down + Vector2.left):

                break;

        }

        return targetDirection;
    }
    #endregion

    #region Gamepad Movement
    void GamepadMovement()
    {

    }
    #endregion

    public bool IsMoving
    {
        get
        {
            return input.targetDirection != Vector3.zero;
        }
    }
}
