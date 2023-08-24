using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
[RequireComponent(typeof(SCR_EnemyAnimationController))]
public class SCR_AI_WasabiPea : MonoBehaviour
{
    #region State Machine Declerations
    public SCR_AI_WasabiBaseStates currentState;
    public SCR_AI_WasabiBaseStates idleState = new SCR_AI_Wasabi_IdleState();
    public SCR_AI_WasabiBaseStates movementState = new SCR_AI_Wasabi_MovementState();
    public SCR_AI_WasabiBaseStates attackState = new SCR_AI_Wasabi_AttackState();
    public SCR_AI_WasabiBaseStates explosiveState = new SCR_AI_Wasabi_ExplosiveState();
    public SCR_AI_WasabiBaseStates deathState = new SCR_AI_Wasabi_DeathState();
    #endregion

    #region Getters & Setters
    //public float publicDetectionRange { get; set; }
    //public float publicAttackRange { get; set; }
    public float publicAttackDamage { get; set; }
    public float publicExplosiveDamage { get; set; }
    //public LayerMask publicPlayerLayerMask { get; set; }
    //public float publicAttackDelayTimer { get; set; }
    public float publicAttackChargeTimer { get; set; }
    public float publicAttackHangTimer { get; set; }
    public float publicExplosiveTimer { get; set; }
    public bool bSwitchToExplosiveState { get; set; }
    //public GameObject publicDeathParticles { get; set; }
    public GameObject publicExplosionParticles { get; set; }
    public SCR_EnemyStats EnemyStats { get; private set; }
    public SCR_EnemyAnimationController AnimationController { get; private set; }
    public SCR_EnemyAudioManager AudioManager { get; private set; }

    public AnimationClip DeathAnimation { get; private set; }
    #endregion

    #region SerializeField Variables
    /*[Header("Range Settings")]
    [SerializeField] float detectionRange = 15f;
    [SerializeField] float attackRange = 2f;*/

    [Header("Speed Settings")]
    //[SerializeField] float movementSpeed = 2f;
    [SerializeField] float explosiveSpeed = 3f;

    [Header("Damage Settings")]
    [SerializeField] float attackDamage = 5f;
    [SerializeField] float explosiveDamage = 10f;
    //[SerializeField] LayerMask playerLayerMask;

    [Header("Timer Settings")]
    //[SerializeField] float attackDelayTimer = 3f;
    [SerializeField] float attackChargeTimer = 0.5f;
    [SerializeField] float attackHangTimer = 2f;
    [SerializeField] float explosiveTimer = 3f;

    /*[Header("Rigidbody Settings")]
    [SerializeField] float wasabiMass = 0.5f;
    [SerializeField] float wasabiDrag = 2f;
    [SerializeField] float wasabiAngularDrag = 0.05f;*/

    [Header("Other Settings")]
    //[SerializeField] GameObject deathParticlePrefab;
    [SerializeField] GameObject explosionParticles;
    [SerializeField] AnimationClip deathAnim;
    public GameObject puddleObject;
    #endregion

    #region Private Variables
    GameObject gameManager;
    NavMeshAgent meshAgent;
    Rigidbody rb;
    bool bHasStartedExploding = false;
    bool bIsDead = false;
    SCR_EnemyCounter enemyCounter;
    //SCR_EnemyHealth enemyHealth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
        //enemyHealth = GetComponent<SCR_EnemyHealth>();
        EnemyStats = GetComponent<SCR_EnemyStats>();
        AnimationController = GetComponent<SCR_EnemyAnimationController>();
        AudioManager = GetComponent<SCR_EnemyAudioManager>();

        //enemyCounter.numberWasabiEnemies++;

        meshAgent = GetComponent<NavMeshAgent>();
        //meshAgent.speed = movementSpeed;

        /*publicDetectionRange = detectionRange;
        publicAttackRange = attackRange;*/

        publicAttackDamage = attackDamage;
        publicExplosiveDamage = explosiveDamage;
        //publicPlayerLayerMask = playerLayerMask;

        //publicAttackDelayTimer = attackDelayTimer;
        publicAttackChargeTimer = attackChargeTimer;
        publicAttackHangTimer = attackHangTimer;
        publicExplosiveTimer = explosiveTimer;
        DeathAnimation = deathAnim;

        bSwitchToExplosiveState = false;

        //publicDeathParticles = deathParticlePrefab;
        publicExplosionParticles = explosionParticles;

        rb = GetComponent<Rigidbody>();
        /*rb.mass = wasabiMass;
        rb.drag = wasabiDrag;
        rb.angularDrag = wasabiAngularDrag;*/

        if(EnemyStats.UseNewMovement)
        {
            currentState = idleState;
            currentState.StartState(gameObject, meshAgent);
        }
        else
        {
            currentState = movementState;
            currentState.StartState(gameObject, meshAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.F))
        {
            bSwitchToExplosiveState = true;
        }*/

        if(EnemyStats.CurrentHealth <= 0 && !bIsDead)
        {
            bIsDead = true;
            //Switch to death state
            currentState = deathState;
            currentState.StartState(gameObject, meshAgent);
        }
        if(bSwitchToExplosiveState && !bIsDead && !bHasStartedExploding)
        {
            bHasStartedExploding = true;
            meshAgent.speed = explosiveSpeed;
            currentState = explosiveState;
            currentState.StartState(gameObject, meshAgent);
        }

        currentState.UpdateState(gameObject, meshAgent);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(gameObject, meshAgent);
    }

    public void StartWasabiCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
