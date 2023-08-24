using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PSM_InputHandler : MonoBehaviour
{
    public Vector2 CheckKBInput;
    public Vector2 mouseWheelInput; 
    public PlayerInput playerInput;
    //private SCR_WeaponManager weaponManager;
    private SCR_WeaponInventory weaponInventory;
    private SCR_WeaponHandler weaponHandler;
    private Animator playerAnimator;

    //Variables to store each related input action from the player actions input action map
    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction mouseWheelAction;
    public InputAction basicAttackAction;
    public InputAction abilityOneAction;
    public InputAction abilityTwoAction;
    public InputAction abilityThreeAction;


    //Sorry I needed to put this here - Joe
    public InputAction foodOrderToggleAction;
    public InputAction pauseMenuToggleAction;
    public SCR_ThoughtBubble thoughtBubble;
    public SCR_PauseMenu pauseMenu;

    private bool isAttacking;

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }

    #region Delegates
    public delegate void _delegate(string bar);
    public static _delegate jumpDelegate;

    public static _delegate _basicAttackDelegate;
    public static _delegate _abilityOneDelegate;
    public static _delegate _abilityTwoDelegate;
    public static _delegate _abilityThreeDelegate;
    #endregion

    #region MouseMovement
    [Header("Mouse Input")]
    public Vector2 mousePosition;

    [SerializeField] Vector3 worldPosition;
    [SerializeField] LayerMask walkableLayer;
    Ray ray;
    #endregion

    #region KeyboardMovement

    [Header("Keyboard Input")]
    [SerializeField] public Vector3 targetDirection = Vector3.zero;
    Vector3 relativeUp = new Vector3(1, 0, -1);
    Vector3 relativeDown = new Vector3(-1, 0, 1);
    Vector3 relativeRight = new Vector3(-1, 0, -1);
    Vector3 relativeLeft = new Vector3(1, 0, 1);

    //Vector3 relativeUpRight = new Vector3(0.707107f, 0, 0.707107f);
    //Vector3 relativeDownRight = new Vector3(0.707107f, 0, -0.707107f);
    //Vector3 relativeUpLeft = new Vector3(-0.707107f, 0, -0.707107f);
    //Vector3 relativeDownLeft = new Vector3(-0.707107f, 0, 0.707107f);

    void ConvertKeyboardInputValues()
    {
        //Handling the cardinal input direction
        if (CheckKBInput == Vector2.zero)
        {
            targetDirection = Vector3.zero;
        }

        if (CheckKBInput == new Vector2(0, 1)) //W key is pressed
        {
            targetDirection = relativeUp;
        }

        if (CheckKBInput == new Vector2(-1, 0)) //A key is pressed
        {
            targetDirection = relativeLeft;
        }

        if (CheckKBInput == new Vector2(0, -1)) //S key is pressed
        {
            targetDirection = relativeDown;
        }

        if (CheckKBInput == new Vector2(1, 0)) //D key is pressed
        {
            targetDirection = relativeRight;
        }

        //Handling combinations of cardinal direciton
        if (CheckKBInput == new Vector2(0.707107f, 0.707107f)) //W and D keys are pressed
        {
            targetDirection = (relativeUp + relativeRight).normalized;
        }

        if (CheckKBInput == new Vector2(-0.707107f, 0.707107f)) //W and A keys are pressed
        {
            targetDirection = (relativeUp + relativeLeft).normalized;
        }

        if (CheckKBInput == new Vector2(-0.707107f, -0.707107f)) //S and A keys are pressed
        {
            targetDirection = (relativeDown + relativeLeft).normalized;
        }

        if (CheckKBInput == new Vector2(0.707107f, -0.707107f)) //S and D keys are pressed
        {
            targetDirection = (relativeDown + relativeRight).normalized;
        }

    }

    #endregion

    #region Gamepad Movement

    #endregion

    private void Start()
    {
        //Get the player input component
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
        weaponHandler = GetComponent<SCR_WeaponHandler>();
        //weaponManager = GetComponent<SCR_WeaponManager>();

        //Bind each of the defined input actions to the appropriate mapping in the input action map
        bindInputActions();
        EnableInputActions();
        GetInputFromActionMap();
    }

    void Update()
    {
        mousePosition = Input.mousePosition;
    }

    public Vector3 MousePositionToWorldPosition()
    {
        Vector3 mousePos = mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, walkableLayer))
        {
            worldPosition = hit.point;
            worldPosition.y = Mathf.RoundToInt(worldPosition.y);
        }

        return worldPosition;
    }

    void bindInputActions()
    {
        moveAction = playerInput.actions["PlayerMovement"];
        mouseWheelAction = playerInput.actions["MouseScroll"];
        basicAttackAction = playerInput.actions["BasicAttack"];
        abilityOneAction = playerInput.actions["AbilityOne"];
        abilityTwoAction = playerInput.actions["AbilityTwo"];
        abilityThreeAction = playerInput.actions["AbilityThree"];

        foodOrderToggleAction = playerInput.actions["FoodOrderToggle"];
        pauseMenuToggleAction = playerInput.actions["PauseMenuToggle"];
    }

    void EnableInputActions()
    {
        moveAction.Enable();
        mouseWheelAction.Enable();
        basicAttackAction.Enable();
        abilityOneAction.Enable();
        abilityTwoAction.Enable();
        abilityThreeAction.Enable();
        foodOrderToggleAction.Enable();
        pauseMenuToggleAction.Enable();
    }

    void DisableInputActions()
    {
        /*moveAction.Disable();
        mouseWheelAction.Disable();
        basicAttackAction.Disable();
        abilityOneAction.Disable();
        abilityTwoAction.Disable();
        abilityThreeAction.Disable();
        foodOrderToggleAction.Disable();
        pauseMenuToggleAction.Disable();*/

        //playerInput.currentActionMap.RemoveAllBindingOverrides();
    }
    
    public void GetInputFromActionMap()
    {
        SCR_WeaponInventory weaponInventory = GetComponent<SCR_WeaponInventory>();
        #region MoveAction
        moveAction.performed += ctx =>
        {
            CheckKBInput = ctx.ReadValue<Vector2>();
            ConvertKeyboardInputValues();
            GetComponent<Animator>().SetBool("isMoving", true);
        };

        moveAction.canceled += ctx =>
        {
            CheckKBInput = Vector2.zero;
            ConvertKeyboardInputValues();
            GetComponent<Animator>().SetBool("isMoving", false);
        };

        #endregion

        #region Mouse Wheel Action
        mouseWheelAction.performed += ctx => mouseWheelInput = ctx.ReadValue<Vector2>();
        mouseWheelAction.canceled += ctx => mouseWheelInput = Vector2.zero;
        #endregion

        #region Basic Attack Action
        basicAttackAction.performed += ctx =>
        {
            if (weaponHandler.BWeaponOnCooldown[0] || isAttacking)
                return;

            weaponHandler.CallCooldownRoutine(0);

            weaponHandler.UpdateWeaponIndex(0);
            weaponHandler.CurrentWeapon.SetWeaponActive();
            weaponHandler.CurrentWeapon.PlayAttackAnimation();

            isAttacking = true;
        };
        #endregion

        #region Ability One Action

        abilityOneAction.performed += ctx =>
        {
            if (weaponHandler.BWeaponOnCooldown[1] || isAttacking)
                return;

            weaponHandler.CallCooldownRoutine(1);
           
            weaponHandler.UpdateWeaponIndex(1);
            weaponHandler.CurrentWeapon.SetWeaponActive();
            weaponHandler.CurrentWeapon.PlayAttackAnimation();
            weaponHandler.CurrentWeapon.DrawAttackArea();

            isAttacking = true;
        };
        #endregion

        #region Ability Two Action
        abilityTwoAction.performed += ctx =>
        {
            if (weaponHandler.BWeaponOnCooldown[2] || isAttacking)
                return;

            weaponHandler.CallCooldownRoutine(2);

            weaponHandler.UpdateWeaponIndex(2);
            weaponHandler.CurrentWeapon.SetWeaponActive();
            weaponHandler.CurrentWeapon.PlayAttackAnimation();
            weaponHandler.CurrentWeapon.DrawAttackArea();
            
            isAttacking = true;
        };
        #endregion

        #region Ability Three Action
        abilityThreeAction.performed += ctx =>
        {
            if (weaponHandler.BWeaponOnCooldown[3] || isAttacking)
                return;

            weaponHandler.CallCooldownRoutine(3);

            weaponHandler.UpdateWeaponIndex(3);
            weaponHandler.CurrentWeapon.SetWeaponActive();
            weaponHandler.CurrentWeapon.PlayAttackAnimation();
            weaponHandler.CurrentWeapon.DrawAttackArea();

            isAttacking = true;
        };
        #endregion

        #region FoodOrderToggle
        foodOrderToggleAction.performed += ctx =>
        {
            pauseMenu.ToggleQuickAccessMenu();
        };
        #endregion

        #region PauseMenuToggle
        pauseMenuToggleAction.performed += ctx =>
        {
            pauseMenu.TogglePauseMenu();
        };
        #endregion
    }

    bool IsWeaponAnimationPlaying()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_MortarPestleAttack") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_SpoonAttack") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_FryingPanAttack") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_MeatTenderizer") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_BlenderAttack"))
        {
            return true;
        } 
        else
        {
            return false;
        }
            
    }

    private void OnDestroy()
    {
        //DisableInputActions();
    }
}
