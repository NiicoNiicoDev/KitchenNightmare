using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
[RequireComponent(typeof(SCR_EnemyAnimationController))]
[RequireComponent(typeof(SCR_EnemyAudioManager))]
public class SCR_AI_RiceGrain : MonoBehaviour
{
    #region State Decleration
    public SCR_AI_RiceBaseStates currentState;
    public SCR_AI_RiceBaseStates idleState = new SCR_AI_Rice_IdleState();
    public SCR_AI_RiceBaseStates movementState = new SCR_AI_Rice_MovementState();
    public SCR_AI_RiceBaseStates attack1State = new SCR_AI_Rice_Attack1State();
    public SCR_AI_RiceBaseStates attack2State = new SCR_AI_Rice_Attack2State();
    public SCR_AI_RiceBaseStates deathState = new SCR_AI_Rice_DeathState();
    #endregion

    #region Variables
    [Header("Range Values")]
    /*[SerializeField] float localDetectionRange = 10f;
    [SerializeField] float localAttackRange = 3f;*/
    [SerializeField] float localChargeDistance = 10f;

    [Header("General Settings")]
    /*[SerializeField] float movementSpeed = 5f;
    [SerializeField] GameObject localDeathParticles;
    [SerializeField] LayerMask localPlayerLayerMask;*/
    [SerializeField] AnimationClip deathAnim;

    /*[Header("Rigidbody Settings")]
    [SerializeField] float riceMass = 0.5f;
    [SerializeField] float riceDrag = 2f;
    [SerializeField] float riceAngularDrag = 0.05f;*/

    [Header("Timer Values")]
    //[SerializeField] float localAttackDelayTimer = 2f;
    [SerializeField] float localChargeTimer = 3f;

    [Header("Damage Values")]
    [SerializeField] int localAttack1Damage = 3;
    [SerializeField] int localAttack2Damage = 6;

    NavMeshAgent meshAgent;
    Rigidbody rb;
    bool bIsDead = false;
    GameObject gameManager;
    SCR_EnemyCounter enemyCounter;
    #endregion

    #region Getters & Setters
    //public float detectionRange { get; set; }
    //public float attackRange { get; set; }
    public float chargeDistance { get; set; }
    //public float attackDelayTimer { get; set; }
    public float chargeTimer { get; set; }
    //public GameObject deathParticles { get; set; }
    public int attack1Damage { get; set; }
    public int attack2Damage { get; set; }
    //public LayerMask playerLayerMask { get; set; }
    public SCR_EnemyStats EnemyStats { get; private set; }
    public SCR_EnemyAnimationController AnimationController { get; private set; }
    public SCR_EnemyAudioManager AudioManager { get; private set; }
    public AnimationClip DeathAnimation { get; private set; }
    #endregion

    

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        /*detectionRange = localDetectionRange;
        attackRange = localAttackRange;*/
        chargeDistance = localChargeDistance;
        chargeTimer = localChargeTimer;
        //meshAgent.speed = movementSpeed;

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
        EnemyStats = GetComponent<SCR_EnemyStats>();
        AnimationController = GetComponent<SCR_EnemyAnimationController>();
        AudioManager = GetComponent<SCR_EnemyAudioManager>();
        DeathAnimation = deathAnim;

        rb = GetComponent<Rigidbody>();
        /*rb.mass = riceMass;
        rb.drag = riceDrag;
        rb.angularDrag = riceAngularDrag;*/

        //attackDelayTimer = localAttackDelayTimer;

        //deathParticles = localDeathParticles;
        attack1Damage = localAttack1Damage;
        attack2Damage = localAttack2Damage;
        //playerLayerMask = localPlayerLayerMask;

        //enemyCounter.numberRiceEnemies++;
        currentState = idleState;
        currentState.StartState(gameObject, meshAgent);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.F2))
        {
            EnemyStats.TakeDamage(EnemyStats.CurrentHealth);
        }*/

        if(EnemyStats.CurrentHealth <= 0 && !bIsDead)
        {
            bIsDead = true;
            //Enemy is dead, switch to death state
            currentState = deathState;
            currentState.StartState(gameObject, meshAgent);
        }

        currentState.UpdateState(gameObject, meshAgent);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(gameObject, meshAgent);
    }

    public void StartRiceCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
