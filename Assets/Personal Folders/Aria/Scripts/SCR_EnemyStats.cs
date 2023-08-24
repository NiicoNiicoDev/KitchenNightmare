using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(SCR_EnemyDeathFade))]
//This script contains all the generic values that are needed for the enemies; access this script to alter health and stun the enemy
public class SCR_EnemyStats : MonoBehaviour
{
/*    [Header("Test Variables")]
    [Tooltip("Use the hotkey to test stunning on this enemy, hotkey is F5")]
    [SerializeField] bool bUseTestingHotkey = false;
    [Tooltip("Duration of the test stun")]
    [SerializeField] float testStunDurtaion = 1f;
    [Tooltip("Use the new movement system for this enemy, disabled by default until finished")]
    [SerializeField] bool _useNewMovemnt = false;*/

    [Header("General Stats")]
    [Tooltip("The time after being stunned until the enemy can be stunned again, this prevents stun-locking")]
    [SerializeField] int _enemyHealth = 10;
    [SerializeField] int _enemyFOV = 75;
    [SerializeField] GameObject healthSliderObject;
    
    [Range(0, 200)]
    [SerializeField] int _enemyDamageModifier = 1;
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] LayerMask _enemyLayerMask;
    [SerializeField] float movementSpeed;

    [Header("Particle Objects")]
    [SerializeField] GameObject _deathParticles;
    [SerializeField] GameObject stunParticles;
    [SerializeField] GameObject damageParticles;
    [SerializeField] GameObject _aoeObject;
    [SerializeField] Vector3 damageParticlePosition;

    [Header("Range Stats")]
    [SerializeField] float _chaseRange = 15f;
    [SerializeField] float _detectionRange;
    [SerializeField] float _attackRange;

    [Header("Timer / Delay Stats")]
    [SerializeField] float damageFlashDuration = 0.2f;
    [SerializeField] float stunInvulWindow = 1f;
    [SerializeField] float _attackDelay;

    [Header("Rigidbody Stats")]
    [SerializeField] float mass;
    [SerializeField] float drag;
    [SerializeField] float angularDrag;


    float stunDuration;
    float windowTimer;
    float flashDurationTimer;
    bool bCanBeStunned = true;
    bool bHasFlashed = false;
    Slider healthSlider;
    Rigidbody rb;
    NavMeshAgent meshAgent;

    [HideInInspector] public SCR_ScoringSystem scoringSystem;
    [HideInInspector] public bool justDamaged = false;

    #region Properties
    public int CurrentHealth
    {
        get
        {
            return _enemyHealth;
        }
        set
        {
            _enemyHealth = value;
        }
    }

    public int EnemyDamageMod
    {
        get
        {
            return _enemyDamageModifier;
        }
        set
        {
            _enemyDamageModifier = value;
        }
    }

    public bool UseNewMovement { get; private set; } = true;

    public float ChaseRange
    {
        get
        {
            return _chaseRange;
        }
    }

    public int EnemyFOV
    {
        get
        {
            return _enemyFOV;
        }
    }

    public GameObject EnemyDeathParticles
    {
        get
        {
            return _deathParticles;
        }
    }

    public GameObject AOEObject
    {
        get
        {
            return _aoeObject;
        }
    }

    public LayerMask PlayerLayerMask
    {
        get
        {
            return _playerLayerMask;
        }
    }

    public LayerMask EnemyLayerMask
    {
        get
        {
            return _enemyLayerMask;
        }
    }

    public float DetectionRange
    {
        get
        {
            return _detectionRange;
        }
    }

    public float AttackRange
    {
        get
        {
            return _attackRange;
        }
    }

    public float AttackDelay
    {
        get
        {
            return _attackDelay;
        }
    }


    public bool IsStunned { get; private set; }
    public SCR_PlayerStats PlayerStats { get; private set; }
    public SCR_EnemyDeathFade DeathFade { get; private set; }
    public bool bIsPartOfAWave { get; private set; }
    public bool bSpawnedByBoss { get; set; }
    #endregion

    SCR_EnemySpawner enemySpawner = null;
    bool bIsBoss;
    SkinnedMeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerStats>();
        meshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        windowTimer = stunInvulWindow;
        meshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        meshAgent.speed = movementSpeed;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;

        if (this.gameObject.CompareTag("Boss"))
        {
            bIsBoss = true;
        }
        else
        {
            healthSlider = healthSliderObject.GetComponent<Slider>();
            healthSlider.maxValue = CurrentHealth;
            healthSlider.value = CurrentHealth;
        }

        DeathFade = GetComponent<SCR_EnemyDeathFade>();
        flashDurationTimer = damageFlashDuration;
    }

    // Update is called once per frame
    void Update()
    {

        if(bHasFlashed)
        {
            flashDurationTimer -= Time.deltaTime;
            if(flashDurationTimer < 0)
            {
                flashDurationTimer = damageFlashDuration;
                meshRenderer.material.color = Color.white;
                bHasFlashed = false;
            }
        }

        //If the enemy is stunned, decrease the timer until 0 then end the stun
        if (IsStunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0f)
            {
                //Enemy is no longer stunned
                IsStunned = false;
                if(stunParticles) stunParticles.SetActive(false);

                //Debug.Log("Stun Ended");
            }
        }

        if (!IsStunned && !bCanBeStunned)
        {
            //The enemy is no longer stunned but cannot be stunned again until the timer reaches 0
            windowTimer -= Time.deltaTime;
            if (windowTimer <= 0f)
            {
                bCanBeStunned = true;
                windowTimer = stunInvulWindow;
                //Debug.Log("Can be stunned again");
            }
        }

        //This is only used to test the stun in instances where a player or weapon is not in the scene
        /*if (bUseTestingHotkey && Input.GetKeyDown(KeyCode.F5))
        {
            //StunEnemy(testStunDurtaion);
            //TakeDamage(CurrentHealth);
        }*/
    }

    private void LateUpdate()
    {
        if(!bIsBoss)
        {
            healthSliderObject.transform.rotation = Camera.main.transform.rotation; //Code from https://forum.unity.com/threads/ui-elements-face-to-camera.359550/
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        justDamaged = true;

        meshRenderer.material.color = Color.red;
        bHasFlashed = true;

        if(damageParticles)
        {
            GameObject _object = Instantiate(damageParticles, gameObject.transform);
            _object.transform.localPosition = damageParticlePosition;

            Destroy(_object, 0.5f);
        }

        if(!bIsBoss)
        {
            healthSlider.value = CurrentHealth;

        }

        if (CurrentHealth <= 0)
        {
            healthSliderObject.SetActive(false);
        }
    }

    public void SpawnInWave(SCR_EnemySpawner spawner)
    {
        //Debug.Log("In Wave");
        bIsPartOfAWave = true;
        enemySpawner = spawner;
    }

    public void StunEnemy(float duration)
    {
        if (!IsStunned && bCanBeStunned) //If the enemy is not stunned but can be then run the logic to begin the stun
        {
            //Debug.Log("Stunned");
            bCanBeStunned = false;
            stunDuration = duration;
            IsStunned = true;
            if(stunParticles)
            {
                stunParticles.SetActive(true);
                stunParticles.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    private void OnDestroy()
    {
        if(bIsPartOfAWave)
        {
            enemySpawner.EnemyKilled();
        }
    }
}
