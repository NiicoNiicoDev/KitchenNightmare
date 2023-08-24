using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SCR_EnemyStats))]
public class SCR_AI_RottenSalmonChunk : MonoBehaviour
{
    #region SALMON CHUNK STATES
    private SCR_AI_RSC_BaseState currentState;

    private SCR_AI_RSC_BaseState idle = new SCR_AI_RSC_Idle();
    private SCR_AI_RSC_BaseState moving = new SCR_AI_RSC_Movement();
    private SCR_AI_RSC_BaseState wormProjectileAttack = new SCR_AI_RSC_WormProjectileAttack();
    private SCR_AI_RSC_BaseState stunCloudAttack = new SCR_AI_RSC_StunCloudAttack();
    [HideInInspector] public SCR_AI_RSC_BaseState flee = new SCR_AI_RSC_Flee();
    private SCR_AI_RSC_BaseState death = new SCR_AI_RSC_Death();
    #endregion

    #region SALMON CHUNK DETAILS
    public float movementSpeed = 1f;

    public float fleeTime = 1f;

    [SerializeField] private float detectionRange = 1f;

    [SerializeField] private float stunAttackRange = 1f;

    [SerializeField] private float stunCloudFrequency = 1f;

    [SerializeField] private float wormProjectileFrequency = 1f;

    [SerializeField] private int touchDamage = 1;
    #endregion

    #region OTHER VARIABLES
    public Animator salmonAnimator;

    public GameObject deathParticles;

    public GameObject wormProjectiles;

    public GameObject stunCloudParticles;

    public AnimationClip deathAnimation;

    [HideInInspector] public bool canChangeState = true;

    [HideInInspector] public GameObject player;

    [HideInInspector] public SCR_EnemyStats healthScript;

    [HideInInspector] public Coroutine activeCoroutine;

    [SerializeField] private Transform colliderPosition;

    private NavMeshAgent navMeshAgent;

    private float stunCloudTimer = 0f;

    private float wormProjectileTimer = 0f;

    private Collider collider;

    private SCR_PlayerStats playerHealthScript;
    #endregion

    #region START & UPDATE
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        healthScript = GetComponent<SCR_EnemyStats>();

        collider = GetComponent<Collider>();

        stunCloudParticles.SetActive(false);

        /*collider.transform.position = colliderPosition.position;
        collider.transform.rotation = colliderPosition.rotation;*/

        EnterState(idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            healthScript.TakeDamage(healthScript.CurrentHealth);
        }
        currentState.UpdateState(gameObject, navMeshAgent);

        if (currentState != death)
        {
            transform.LookAt(player.transform);
        }

        stunCloudTimer += Time.deltaTime;
        wormProjectileTimer += Time.deltaTime;

        if (healthScript.CurrentHealth <= 0)
        {
            /*collider.transform.position = colliderPosition.position;
            collider.transform.rotation = colliderPosition.rotation;*/
            EnterState(death);
            /*if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), transform.localScale.y))
            {
                transform.position += transform.TransformDirection(Vector3.forward) * 0.1f;
                
            }*/
            return;
        }
        else if (healthScript.justDamaged)
        {
            EnterState(idle);
            salmonAnimator.SetTrigger("Damaged");
            healthScript.justDamaged = false;
        }

        if (healthScript.IsStunned)
        {
            EnterState(idle);
            return;
        }

        if (canChangeState)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= stunAttackRange && stunCloudTimer > stunCloudFrequency)
            {
                canChangeState = false;
                stunCloudTimer = 0f;
                EnterState(stunCloudAttack);
                return;
            }

            if(wormProjectileTimer > wormProjectileFrequency && currentState == moving)
            {
                RaycastHit hit;
                //stops salmon chunk firing missiles when near walls
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 2f))
                {
                    if (hit.collider.gameObject.name.Contains("Wall"))
                    {
                        return;
                    }
                }
                canChangeState = false;
                wormProjectileTimer = 0f;
                EnterState(wormProjectileAttack);
                return;
            }

            if(Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                EnterState(moving);
                return;
            }

            EnterState(idle);
        }
    }
    #endregion

    #region CHANGE STATE
    public void EnterState(SCR_AI_RSC_BaseState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        if (currentState != null)
        {
            currentState.ExitState(gameObject, navMeshAgent);
        }

        currentState = newState;
        currentState.StartState(gameObject, navMeshAgent);
    }
    #endregion

    #region IENUMERATOR CALL
    public void StartRSCCoroutine(IEnumerator coroutine)
    {
        activeCoroutine = StartCoroutine(coroutine);
    }

    public void CancelRSCCoroutine()
    {
        StopCoroutine(activeCoroutine);
    }
    #endregion

    #region ON COLLISION ENTER
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (!healthScript.IsStunned)
            {
                playerHealthScript = collision.collider.GetComponent<SCR_PlayerStats>();
                //calling coroutine so it can be stopped when required
                playerHealthScript.TakeDamage(touchDamage);
            }
        }
    }
    #endregion
}
