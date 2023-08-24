using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SCR_EnemyStats))]
public class SCR_AI_RNS : MonoBehaviour
{
    #region NORI SHEET STATES
    private SCR_AI_RNS_BaseStates currentState;
    private SCR_AI_RNS_BaseStates idle = new SCR_AI_RNS_Idle();
    private SCR_AI_RNS_BaseStates moving = new SCR_AI_RNS_Movement();
    private SCR_AI_RNS_BaseStates attack = new SCR_AI_RNS_Attack();
    [HideInInspector] public SCR_AI_RNS_BaseStates flee = new SCR_AI_RNS_Flee();
    [HideInInspector] public SCR_AI_RNS_BaseStates damaged = new SCR_AI_RNS_Damaged();
    private SCR_AI_RNS_BaseStates death = new SCR_AI_RNS_Death();
    #endregion

    #region NORI SHEET DETAILS
    public float noriSpeed = 1f;

    public float attackRadius = 1f;

    public int attackDamage = 1;

    public float fleeTime = 1f;

    [HideInInspector] public bool canMultiply = true;

    [HideInInspector] public bool leader = true;

    [SerializeField] private float detectionRange = 1f;

    [SerializeField] private float attackRange = 1f;
    #endregion

    #region OTHER VARIABLES
    public Animator noriAnimator;

    public AnimationClip deathAnimation;

    public LayerMask playerLM;

    public GameObject deathParticles;

    [HideInInspector] public bool canChangeState = true;

    [HideInInspector] public GameObject player;

    [HideInInspector] public GameObject leadNori;

    [HideInInspector] public SCR_EnemyStats healthScript;

    private NavMeshAgent navMeshAgent;

    private bool disableStateChanges = false;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        healthScript = GetComponent<SCR_EnemyStats>();

        EnterState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(!leader && leadNori.IsDestroyed())
        {
            Destroy(gameObject);
        }

        currentState.UpdateState(gameObject, navMeshAgent);
        if (currentState != flee && currentState != death)
        {
            transform.LookAt(player.transform);
        }

        if(healthScript.CurrentHealth <= 0)
        {
            EnterState(death);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), transform.localScale.y))
            {
                transform.position += transform.TransformDirection(Vector3.back) * 0.1f;

            }
            disableStateChanges = true;
            return;
        } else if (healthScript.justDamaged)
        {
            healthScript.justDamaged = false;
            EnterState(damaged);
            return;
        }

        if(healthScript.IsStunned)
        {
            EnterState(idle);
            return;
        }

        if (canChangeState)
        {
            if(Vector3.Distance(transform.position, player.transform.position) <= attackRange && currentState == moving)
            {
                EnterState(attack);
                canChangeState = false;
                return;
            } else if(Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                EnterState(moving);
                return;
            }
            else
            {
                EnterState(idle);
            }
        }
    }

    public void EnterState(SCR_AI_RNS_BaseStates newState)
    {
        if (!disableStateChanges)
        {
            if (newState == currentState)
            {
                return;
            }

            currentState = newState;
            currentState.StartState(gameObject, navMeshAgent);
        }
    }

    public void StartRNSCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
