using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
public class SCR_AI_RottenRiceGrain : MonoBehaviour
{
    #region RICE GRAIN STATES
    private SCR_AI_RRG_BaseStates currentState;
    [HideInInspector] public SCR_AI_RRG_BaseStates idle = new SCR_AI_RRG_IdleState();
    private SCR_AI_RRG_BaseStates moving = new SCR_AI_RRG_Movement();
    private SCR_AI_RRG_BaseStates attack = new SCR_AI_RRG_Attack();
    private SCR_AI_RRG_BaseStates stick = new SCR_AI_RRG_Sticking();
    private SCR_AI_RRG_BaseStates death = new SCR_AI_RRG_Death();
    #endregion

    #region RICE GRAIN DETAILS
    [Header("Rotten Rice Grain Details")]
    //attack damage per hit, NOT total damage during stick time and doesn't include explosion damage
    public int attackDamage = 1;

    public int explosionDamage = 1;

    //how frequently the rice grain does damage while it is stuck to the player
    public float damageDelay = 1f;

    public float stickTime = 1f;

    public float swoopSpeed = 1f;

    public float movementSpeed = 1f;

    //how much the rice grain slows the player down by when sticking to the player
    public int slowdownPercentage = 1;

    [SerializeField] private float lOSRadius = 1f;

    [SerializeField] private float detectionRange = 1f;

    [SerializeField] private float attackRange = 1f;
    #endregion

    #region OTHER VARIABLES
    [Header("Other Variables")]
    public Animator riceGrainAnimator;

    public AnimationClip deathAnimation;

    public GameObject explosionParticles;

    public GameObject deathParticles;

    //offset from center of player the rice grain should be when stuck to the player
    public Vector3 stickOffset = new Vector3(1f, 1f, 1f);

    public Transform deathParticleSpawn;

    [HideInInspector] public GameObject player;

    [HideInInspector] public SCR_PlayerStats playerHealthScript;

    [HideInInspector] public float timeSinceLastAttack = 0f;

    [HideInInspector] public float attackLength = 0f;

    [HideInInspector] public bool unstick = false;

    [SerializeField] private LayerMask playerLM;

    [SerializeField] private float attackFrequency = 1f; //must be larger than swoop animation clip length to avoid bugs
    
    [HideInInspector] public SCR_EnemyStats healthScript;

    private NavMeshAgent navMeshAgent;

    private RaycastHit hit;

    private float attackTimer = 0f;

    //ENEMY health script
    //private SCR_EnemyStats healthScript;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        playerHealthScript = player.GetComponent<SCR_PlayerStats>();

        healthScript = GetComponent<SCR_EnemyStats>();

        timeSinceLastAttack = attackFrequency;

        EnterState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (currentState != death)
        {
            transform.LookAt(player.transform.position);
        }

        currentState.UpdateState(gameObject, navMeshAgent);

        if(healthScript.CurrentHealth <= 0)
        {
            EnterState(death);
            return;
        }

        if (healthScript.justDamaged)
        {
            healthScript.justDamaged = false;
            if (currentState == stick)
            {
                unstick = true;
            }
        }

        if (healthScript.IsStunned)
        {
            EnterState(idle);
            return;
        }

        if (currentState != attack || attackTimer > attackLength)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackRange && (currentState == moving || currentState == attack) && timeSinceLastAttack > attackFrequency)
            {
                attackTimer = 0f;
                EnterState(attack);
                return;
            }

            if (Vector3.Distance(transform.position, player.transform.position) < detectionRange && (currentState == moving || currentState == idle) && timeSinceLastAttack > attackFrequency)
            {
                EnterState(moving);
                return;
            }

            if (currentState != stick)
            {
                EnterState(idle);
            }
        }
    }

    public void EnterState(SCR_AI_RRG_BaseStates newState)
    {
        if(newState == currentState)
        {
            return;
        }

        currentState = newState;
        currentState.StartState(gameObject, navMeshAgent);
    }

    public void StartRRGCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (timeSinceLastAttack > attackFrequency && currentState != death && !healthScript.IsStunned)
        {
            if (collision.collider.CompareTag("Player"))
            {
                EnterState(stick);
            }
        }
    }
}
