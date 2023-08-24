using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RottenWasabiPea : MonoBehaviour
{
    #region WASABI PEA STATES
    private SCR_AI_RottenWasabiPea_BaseStates currentState;
    private SCR_AI_RottenWasabiPea_BaseStates moving = new SCR_AI_RottenWasabiPea_MovementState();
    private SCR_AI_RottenWasabiPea_BaseStates idle = new SCR_AI_RWP_Idle();
    private SCR_AI_RottenWasabiPea_BaseStates attack = new SCR_AI_RWP_Attack();
    private SCR_AI_RottenWasabiPea_BaseStates damaged = new SCR_AI_RWP_Damaged();
    private SCR_AI_RottenWasabiPea_BaseStates death = new SCR_AI_RWP_Death();
    #endregion

    #region ROTTEN WASABI DETAILS
    [Header(header:"Wasabi Details")]
    public float attackPower = 1f;

    public float attackFrequency = 1f;

    //out of 100
    public int chanceOfMultiply = 1;

    [HideInInspector] public bool canMultiply = true;
    
    //line of sight radius; higher values will make it easier for the AI to find the player and vice versa.
    [SerializeField] private float lOSRadius = 1f;

    [SerializeField] private float detectionRange = 1f;

    [SerializeField] private float attackRange = 1f;

    [SerializeField] private int attackDamage = 1;

    [SerializeField] private float poisonFrequency = 1f;

    [SerializeField] private int poisonDamage = 1;

    [SerializeField] private int totalPoisonHits = 1;
    #endregion

    #region VARIABLES
    [Header(header:"Other Variables")]
    public Animator wasabiAnimator;

    public GameObject deathParticles;

    [HideInInspector] public GameObject player;
    
    [HideInInspector] public SCR_EnemyStats healthScript;
    
    //player layer mask
    [SerializeField] private LayerMask playerLM;

    private NavMeshAgent navMeshAgent;

    private RaycastHit hit;

    private SCR_PoisonMechanics poisonScript;

    private float timeSinceAttack = 0f;
    
    #endregion

    #region START & UPDATE
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        healthScript = GetComponent<SCR_EnemyStats>();

        if (!healthScript)
        {
            Debug.Log("Missing health script");
        }

        EnterState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(gameObject, navMeshAgent);

        timeSinceAttack += Time.deltaTime;

        if(healthScript.CurrentHealth <= 0)
        {
            EnterState(death);
            return;
        }

        if (healthScript.justDamaged)
        {
            EnterState(damaged);
            return;
        }

        if (currentState == moving && Physics.SphereCast(transform.position, lOSRadius, transform.TransformDirection(Vector3.forward), out hit, attackRange, playerLM) && timeSinceAttack > attackFrequency)
        {
            EnterState(attack);
            timeSinceAttack = 0f;
            return;
        }

        if ((currentState == idle || currentState == moving) && Physics.SphereCast(transform.position, lOSRadius, transform.TransformDirection(Vector3.forward), out hit, detectionRange, playerLM))
        {
            EnterState(moving);
            return;
        }
        
        EnterState(idle);
        
    }
    #endregion

    void EnterState(SCR_AI_RottenWasabiPea_BaseStates newState)
    {
        if(currentState == newState)
        {
            return;
        }

        currentState = newState;
        currentState.StartState(gameObject, navMeshAgent);
    }

    #region ON COLLISION ENTER
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player");

            poisonScript = collision.gameObject.GetComponent<SCR_PoisonMechanics>();
            if (!poisonScript.bIsPoisoned)
            {
                poisonScript.StartPoisoning(poisonFrequency, poisonDamage, totalPoisonHits);
            }
        }
    }
    #endregion

    public void StartRWPCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
