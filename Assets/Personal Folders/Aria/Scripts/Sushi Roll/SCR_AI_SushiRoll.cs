using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
[RequireComponent(typeof(SCR_EnemyAnimationController))]
[RequireComponent(typeof(SCR_EnemyAudioManager))]
public class SCR_AI_SushiRoll : MonoBehaviour
{
    #region State Declerations
    public SCR_AI_SushiBaseStates currentState;
    public SCR_AI_SushiBaseStates idleState = new SCR_AI_Sushi_IdleState();
    public SCR_AI_SushiBaseStates movementState = new SCR_AI_Sushi_MovementState();
    public SCR_AI_SushiBaseStates wasabiRainState = new SCR_AI_Sushi_WasabiRainState();
    public SCR_AI_SushiBaseStates riceMissilesState = new SCR_AI_Sushi_RiceMissilesState();
    public SCR_AI_SushiBaseStates salmonStompState = new SCR_AI_Sushi_SalmonStompState();
    public SCR_AI_SushiBaseStates noriSpinState = new SCR_AI_Sushi_NoriSpinState();
    public SCR_AI_SushiBaseStates deathState = new SCR_AI_Sushi_DeathState();
    #endregion

    #region Variable Declerations
    [Header("General Values")]
    //[SerializeField] float movementSpeed = 1f;
    [SerializeField] float _spinSpeed = 0.5f;
    [SerializeField] float enrageHealthThreshold = 30f;
    [SerializeField] GameObject _wasabiProjectilePrefab;
    [SerializeField] GameObject _riceMissilePrefab;
    [SerializeField] LayerMask _enemyLayerMask;
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] GameObject _deathParticles;
    [SerializeField] int _numOfRiceMissiles;
    [SerializeField] float _stompJumpForce;
    [SerializeField] float _stompJumpHeight = 5f;
    [SerializeField] GameObject _healthBar;

    [Header("Damage Values")]
    [SerializeField] int _salmonStompDamage = 30;
    [SerializeField] int _noriSpinDamage = 30;
    [SerializeField] int _touchDamage = 1;

    [Header("Range Values")]
    [SerializeField] float _activationRange = 15f;
    [SerializeField] float _maxAttackRange = 5f;

    /*[Header("Rigidbody Values")]
    [SerializeField] float sushiMass = 0.5f;
    [SerializeField] float sushiDrag = 2f;
    [SerializeField] float sushiAngularDrag = 0.05f;*/

    [Header("Timer Values")]
    [SerializeField] float _wakeUpTimer = 2f;
    [SerializeField] float _attackTimer = 2f;
    [SerializeField] float _fireDelay = 1f;
    [SerializeField] float _recoveryTimer = 3f;
    [SerializeField] float _spinDuration = 3f;

    float maxEnemyHealth;
    bool bisDead = false;
    NavMeshAgent meshAgent;
    Rigidbody rb;
    SkinnedMeshRenderer meshRenderer;
    
    Quaternion startingRotation;
    bool startedReset;
    #endregion

    #region Properties
    //General Values
    public Vector3 StartingPosition { get; private set; }
    public Vector3 WorldStartingPos { get; private set; }
    public GameManager GameManager { get; private set; }
    public bool bEnraged { get; set; }
    public bool bIsReady { get; set; }
    public float EnemyHealthPercentage { get; private set; }
    public GameObject WasabiProjectilePrefab { get; private set; }
    public GameObject RiceMissilePrefab { get; private set; }
    public LayerMask EnemyLayerMask { get; private set; }
    public LayerMask PlayerLayerMask { get; private set; }
    public GameObject DeathParticles { get; private set; }
    public int NumOfRiceMissiles { get; private set; }
    public float StompJumpForce { get; private set; }
    public float StompJumpHeight { get; private set; }
    public float SpinSpeed { get; private set; }
    public SCR_EnemyStats EnemyStats { get; private set; }
    public SCR_EnemyAnimationController AnimationController { get; private set; }
    public SCR_EnemyAudioManager AudioManager { get; private set; }
    public GameObject HealthBar { get; private set; }
    public bool bIsAwake { get; set; }

    //Damage Values
    public int SalmonStompDamage { get; private set; }
    public int NoriSpinDamage { get; private set; }

    private int TouchDamage { get { return _touchDamage; } }

    //Range Values
    public float ActivationRange { get; set; }
    public float MaxAttackRange { get; set; }

    //Timer Values
    public float WakeupTimer { get; set; }
    public float AttackTimer { get; set; }
    public float FireDelay { get; set; }
    public float RecoveryTimer { get; private set; }
    public float SpinDuration { get; private set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        EnemyStats = GetComponent<SCR_EnemyStats>();
        AnimationController = GetComponent<SCR_EnemyAnimationController>();
        AudioManager = GetComponent<SCR_EnemyAudioManager>();
        GameManager = GameManager.gameManager;
        //HealthBar = _healthBar;
        //HealthBar.SetActive(false);

        StartingPosition = transform.localPosition;
        WorldStartingPos = transform.position;
        startingRotation = transform.localRotation;

        bIsReady = false;

        if(GameManager != null)
        {
            GameManager.GetComponent<GameManager>();
        }

        #region Properties Set
        {
            //General Values
            maxEnemyHealth = EnemyStats.CurrentHealth;
            WasabiProjectilePrefab = _wasabiProjectilePrefab;
            RiceMissilePrefab = _riceMissilePrefab;
            EnemyLayerMask = _enemyLayerMask;
            PlayerLayerMask = _playerLayerMask;
            DeathParticles = _deathParticles;
            NumOfRiceMissiles = _numOfRiceMissiles;
            StompJumpForce = _stompJumpForce;
            StompJumpHeight = _stompJumpHeight;
            SpinSpeed = _spinSpeed;

            //Damage Values
            SalmonStompDamage = _salmonStompDamage;
            NoriSpinDamage = _noriSpinDamage;

            //Range Values
            ActivationRange = _activationRange;
            MaxAttackRange = _maxAttackRange;

            //Timer Values
            WakeupTimer = _wakeUpTimer;
            AttackTimer = _attackTimer;
            FireDelay = _fireDelay;
            RecoveryTimer = _recoveryTimer;
            SpinDuration = _spinDuration;
        }
        #endregion

        /*meshAgent.speed = movementSpeed;

        rb.mass = sushiMass;
        rb.drag = sushiDrag;
        rb.angularDrag = sushiAngularDrag;*/

        meshRenderer.enabled = false;
        meshAgent.enabled = false;
        rb.useGravity = false;

        currentState = idleState;
        currentState.StartState(gameObject, meshAgent);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(GameManager.bPlayerDead && !startedReset)
        {
            StartCoroutine(ResetBoss());
            startedReset = true;
        }*/

        EnemyHealthPercentage = (EnemyStats.CurrentHealth / maxEnemyHealth) * 100;
        if (EnemyHealthPercentage <= enrageHealthThreshold)
        {
            bEnraged = true;
        }

        if(EnemyStats.CurrentHealth <= 0 && !bisDead) 
        { 
            bisDead = true;
            currentState = deathState;
            currentState.StartState(gameObject, meshAgent);
        }

        /*if(Input.GetKeyDown(KeyCode.F1))
        {
            EnemyStats.TakeDamage(EnemyStats.CurrentHealth);
        }*/

        currentState.UpdateState(gameObject, meshAgent);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(gameObject, meshAgent);
    }

    /*private void OnDestroy()
    {
        //Debug.Log("OnDestroy called");
        if(gameManager != null)
        {
            //gameManager.bossDefeated = true;
        }
    }*/

    //Code adapted from RealSoftGames, 2017
    public void StartCoroutineInScript(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void StopCoroutineInScript(IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }
    //End of adapted code

    public void ResetBoss()
    {
        //yield return new WaitForSeconds(5f);
        //Reset boss position
        transform.localPosition = StartingPosition;
        transform.localRotation = startingRotation;

        //Stop all of the boss' attacks and reset them - Done in each attack state

        //Tell boss that it is neither awake nor ready
        bIsAwake = false;
        bIsReady = false;

        //Reset health
        EnemyStats.CurrentHealth = (int)maxEnemyHealth;

        //Force the boss into its idle state
        meshRenderer.enabled = false;
        meshAgent.enabled = false;
        currentState = idleState;
        currentState.StartState(gameObject, meshAgent);
        startedReset = false;

        FindObjectOfType<SCR_BossHealthBar>().ResetHealthBar();
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
