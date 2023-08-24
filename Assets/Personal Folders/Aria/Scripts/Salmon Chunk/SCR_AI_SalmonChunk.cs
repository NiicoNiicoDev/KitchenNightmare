using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
[RequireComponent(typeof(SCR_EnemyAnimationController))]
[RequireComponent(typeof(SCR_EnemyAudioManager))]
public class SCR_AI_SalmonChunk : MonoBehaviour
{
    #region State Declerations
    public SCR_AI_SalmonBaseStates currentState;
    public SCR_AI_SalmonBaseStates idleState = new SCR_AI_Salmon_IdleState();
    public SCR_AI_SalmonBaseStates movementState = new SCR_AI_Salmon_MovementState();
    public SCR_AI_SalmonBaseStates slapAttackState = new SCR_AI_Salmon_SlapAttackState();
    public SCR_AI_SalmonBaseStates spinAttackState = new SCR_AI_Salmon_SpinAttackState();
    public SCR_AI_SalmonBaseStates strikeAttackState = new SCR_AI_Salmon_StrikeAttackState();
    public SCR_AI_SalmonBaseStates deathState = new SCR_AI_Salmon_DeathState();
    #endregion

    #region Variables
    [Header("General Values")]
    /*[SerializeField] float movementSpeed = 1f;
    [SerializeField] LayerMask _enemyLayerMask;
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] GameObject _deathParticles;*/
    [SerializeField] AnimationClip _deathAnimationClip;

    [Header("Range Values")]
    /*[SerializeField] float _detectionRange = 15f;
    [SerializeField] float _attackRange = 2f;*/
    [SerializeField] float _strikeRange = 10f;

    /*[Header("Rigidbody Values")]
    [SerializeField] float salmonMass = 0.5f;
    [SerializeField] float salmonDrag = 2f;
    [SerializeField] float salmonAngularDrag = 0.05f;*/

    [Header("DamageValues")]
    [SerializeField] float _slapDamage = 15f;
    [SerializeField] float _spinDamage = 25f;
    [SerializeField] float _strikeDamage = 40f;
    [SerializeField] int _touchDamage = 1;

    [Header("Timer Values")]
    //[SerializeField] float _attackDelayTimer = 2f;
    [SerializeField] float _slapRecoveryTimer = 1f;
    [SerializeField] float _slapReadyTimer = 0.5f;
    [SerializeField] float _spinReadyTimer = 1f;
    [SerializeField] float _spinRecoveryTimer = 1f;
    [SerializeField] float _strikeReadyTimer = 2f;
    [SerializeField] float _strikeCooldownTimer = 4f;

    [Header("Strike Attack Values")]
    [SerializeField] float _jumpForce = 25f;
    [SerializeField] float _jumpHeight = 7f;
    [SerializeField] float _diveForce = 50f;

    NavMeshAgent meshAgent;
    Rigidbody rb;
    SCR_EnemyCounter enemyCounter;
    bool bIsDead = false;
    float enemyHealthPercentage;
    #endregion

    #region Getters & Setters
    //General Values
    public float MaxEnemyHealth { get; set; }
    public bool bEnraged { get; set; } = false;
    //public LayerMask EnemyLayerMask { get; set; }
    //public LayerMask PlayerLayerMask { get; set; }
    //public GameObject DeathParticles { get; set; }
    public SCR_EnemyStats EnemyStats { get; private set; }
    public SCR_EnemyAnimationController AnimationController { get; private set; }
    public SCR_EnemyAudioManager AudioManager { get; private set; }
    public AnimationClip DeathAnimation { get; private set; }

    //Range Values
    //public float DetectionRange { get; set; }
    //public float AttackRange { get; set; }
    public float StrikeRange { get; set; }

    //Timer Values
    //public float AttackDelayTimer { get; set; }
    public float SlapRecoveryTimer { get; set; }
    public float SlapReadyTimer { get; set; }
    public float SpinReadyTimer { get; set; }
    public float SpinRecoveryTimer { get; set; }
    public float StrikeReadyTimer { get; set; }
    public float StrikeCooldownTimer { get; set; }

    //Damage Values
    public float SlapDamage { get; set; }
    public float SpinDamage { get; set; }
    public float StrikeDamage { get; set; }

    private int TouchDamage { get { return _touchDamage; } }
    
    //Strike Attack Values
    public float JumpForce { get; set; }
    public float JumpHeight { get; set; }
    public float DiveForce { get; set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //enemyCounter = GameObject.FindGameObjectWithTag("GameController").GetComponent<SCR_EnemyCounter>();
        EnemyStats = GetComponent<SCR_EnemyStats>();
        AnimationController = GetComponent<SCR_EnemyAnimationController>();
        AudioManager = GetComponent<SCR_EnemyAudioManager>();
        DeathAnimation = _deathAnimationClip; 

        /*rb.mass = salmonMass;
        rb.drag = salmonDrag;
        rb.angularDrag = salmonAngularDrag;*/

        //meshAgent.speed = movementSpeed;

        //Setting properties
        {
            MaxEnemyHealth = EnemyStats.CurrentHealth;
            /*EnemyLayerMask = _enemyLayerMask;
            PlayerLayerMask = _playerLayerMask;
            DeathParticles = _deathParticles;

            DetectionRange = _detectionRange;
            AttackRange = _attackRange;*/
            StrikeRange = _strikeRange;

            //AttackDelayTimer = _attackDelayTimer;
            SlapReadyTimer = _slapReadyTimer;
            SlapRecoveryTimer = _slapRecoveryTimer;
            SpinReadyTimer = _spinReadyTimer;
            SpinRecoveryTimer = _spinRecoveryTimer;
            StrikeReadyTimer = _strikeReadyTimer;
            StrikeCooldownTimer = _strikeCooldownTimer;
            
            SlapDamage = _slapDamage;
            SpinDamage = _spinDamage;
            StrikeDamage = _strikeDamage;
            
            JumpForce = _jumpForce;
            JumpHeight = _jumpHeight;
            DiveForce = _diveForce;
        }

        //enemyCounter.numberSalmonEnemies++;
        currentState = idleState;
        currentState.StartState(gameObject, meshAgent);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.F3))
        {
            EnemyStats.TakeDamage(EnemyStats.CurrentHealth);
        }*/

        enemyHealthPercentage = (EnemyStats.CurrentHealth / MaxEnemyHealth) * 100;
        if (enemyHealthPercentage <= 40f)
        {
            bEnraged = true;
        }

        if (EnemyStats.CurrentHealth <= 0 && !bIsDead)
        {
            currentState = deathState;
            currentState.StartState(gameObject, meshAgent);
            bIsDead = true;
        }

        currentState.UpdateState(gameObject, meshAgent);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(gameObject, meshAgent);
    }

    public void StartSalmonCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!EnemyStats.IsStunned)
            {
                collision.collider.GetComponent<SCR_PlayerStats>().TakeDamage(TouchDamage);
            }
        }
    }
}
