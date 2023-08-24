using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
[RequireComponent(typeof(SCR_EnemyAnimationController))]
[RequireComponent(typeof(SCR_EnemyAudioManager))]
public class SCR_AI_NoriSheet : MonoBehaviour
{
    #region State Declerations
    public SCR_AI_NoriBaseState currentState;
    public SCR_AI_NoriBaseState idleState = new SCR_AI_Nori_IdleState();
    public SCR_AI_NoriBaseState movementState = new SCR_AI_Nori_MovementState();
    public SCR_AI_NoriBaseState sweepAttackState = new SCR_AI_Nori_Attack1State();
    public SCR_AI_NoriBaseState slapAttackState = new SCR_AI_Nori_Attack2State();
    public SCR_AI_NoriBaseState deathState = new SCR_AI_Nori_DeathState();
    #endregion

    #region Variables
    [Header("General Settings")]
    /*[SerializeField] float movementSpeed = 2f;
    [SerializeField] LayerMask localEnemyLayerMask;
    [SerializeField] LayerMask localPlayerLayerMask;
    [SerializeField] GameObject localDeathParticles;*/
    [SerializeField] float localSweepAngle = 50f;
    [SerializeField] AnimationClip deathAnimation;

    [Header("Range Values")]
    /*[SerializeField] float localDetectionRange = 15f;
    [SerializeField] float localMaxAttackRange = 8f;*/
    [SerializeField] float localSweepAttackRange = 3f;
    
    /*[Header("Rigidbody Values")]
    [SerializeField] float noriMass = 0.5f;
    [SerializeField] float noriDrag = 2f;
    [SerializeField] float noriAngularDrag = 0.05f;*/

    [Header("Timer Values")]
    //[SerializeField] float localAttackDelayTimer = 3f;
    [SerializeField] float localSweepDelayTimer = 2f;
    [SerializeField] float localSlapDelayTimer = 2f;

    [Header("Damage Values")]
    [SerializeField] float localSlapDamage = 25f;
    [SerializeField] float localSweepDamage = 25f;
    [SerializeField] int _touchDamage = 1;

    NavMeshAgent meshAgent;
    Rigidbody rb;
    GameObject gameManager;
    SCR_EnemyCounter enemyCounter;
    bool bIsDead = false;
    #endregion

    #region Getters & Setters
    //public float detectionRange { get; set; }
    //public float maxAttackRange { get; set; }
    public float sweepAttackRange { get; set; }
    //public float attackDelayTimer { get; set; }
    public float SweepDelayTimer { get; set; }
    public float SlapDelayTimer { get; set; }
    public float slapDamage { get; set; }
    public float sweepDamage { get; set; }

    private int TouchDamage { get { return _touchDamage; } }

    public float SweepAngle { get; set; }
    //public LayerMask EnemyLayerMask { get; set; }
    //public LayerMask PlayerLayerMask { get; set; }
    //public GameObject deathParticles { get; set; }
    public SCR_EnemyStats EnemyStats { get; private set; }
    public SCR_EnemyAnimationController AnimationController { get; private set; }
    public SCR_EnemyAudioManager AudioManager { get; private set; }
    public AnimationClip DeathAnimation { get; private set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
        EnemyStats = GetComponent<SCR_EnemyStats>();
        AnimationController = GetComponent<SCR_EnemyAnimationController>();
        AudioManager = GetComponent<SCR_EnemyAudioManager>();
        DeathAnimation = deathAnimation;

        //meshAgent.speed = movementSpeed;

        /*detectionRange = localDetectionRange;
        maxAttackRange = localMaxAttackRange;*/
        sweepAttackRange = localSweepAttackRange;
        /*attackDelayTimer = localAttackDelayTimer;
        EnemyLayerMask = localEnemyLayerMask;
        deathParticles = localDeathParticles;*/
        SweepAngle = localSweepAngle;
        //PlayerLayerMask = localPlayerLayerMask;
        SweepDelayTimer = localSweepDelayTimer;
        SlapDelayTimer = localSlapDelayTimer;

        slapDamage = localSlapDamage;
        sweepDamage = localSweepDamage;


        /*rb.mass = noriMass;
        rb.drag = noriDrag;
        rb.angularDrag = noriAngularDrag;*/

        /*if(maxAttackRange < sweepAttackRange)
        {
            Debug.LogError("Max Attack Range should never be lower than Sweep Attack Range on Nori Sheet");
            this.enabled = false;
        }*/

        //enemyCounter.numberNoriEnemies++;
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
            currentState = deathState;
            currentState.StartState(gameObject, meshAgent);
        }

        
        currentState.UpdateState(gameObject, meshAgent);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(gameObject, meshAgent);
    }

    public void StartNoriCoroutine(IEnumerator coroutine)
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
