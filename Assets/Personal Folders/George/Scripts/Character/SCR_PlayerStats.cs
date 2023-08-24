using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SCR_PlayerStats : MonoBehaviour
{
    #region Components
    PSM_MovementStateMachine movementStateMachine;
/*    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject[] hearts = new GameObject[20];*/

    [SerializeField] Slider healthBarSlider;
    #endregion

    #region Variables

    //Health
    public int baseHealth = 100;
    public int currentHealth = 0;
    public bool canTakeDamage = true;
    public bool godModeActive = false;

    //Currency
    [SerializeField] int confidenceCurrency = 0;

    //Movement
    [SerializeField] bool isStunned = false;
    [SerializeField] bool isSlowed = false;

    private float baseSpeed;

    [SerializeField] private GameObject gameOverCanvas;

    [SerializeField] private ParticleSystem stunParticles;

    #endregion

    #region Setters and Getters
    public int PlayerHealth { get => currentHealth; }

    public int ConfidenceCurreny { get => confidenceCurrency; }

    public bool IsStunned { get => isStunned; }
    public bool IsSlowed { get => isSlowed; }

    #endregion

    List<Coroutine> DOTRoutines = new List<Coroutine>();

    private void Awake()
    {
        movementStateMachine = GetComponent<PSM_MovementStateMachine>();
        currentHealth = baseHealth;

        healthBarSlider.maxValue = baseHealth;
        healthBarSlider.minValue = 0;

        baseSpeed = movementStateMachine.agent.speed;
        UpdateHealthBarGUI();
    }

    private void Start()
    {
        GetHealthBarIcons();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }*/
    }

    #region Damage Functions

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage || godModeActive) { return; }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }
        
        UpdateHealthBarGUI();
        SCR_AudioManager.instance.Play("SFX_Char_Grunt");
    }

    //Call this from the class that is dealing damage to the player
    public void DamageOverTime(int damage, float lengthOfDOTEffect, float delayBetweenTicksInSeconds)
    {
        StartCoroutine(DamageOverTimeRoutine(damage, lengthOfDOTEffect, delayBetweenTicksInSeconds));   
    }

    public void RemoveDOTS()
    {
        if (DOTRoutines.Count > 0)
        {
            foreach (Coroutine DOTRoutine in DOTRoutines)
            {
                StopCoroutine(DOTRoutine);
            }
        }
    }

    //Don't ever call this, this is just a co-routine for handling the damage over time, the above function is the one you call
    //incase you are calling this from a non monobehaviour class
    public IEnumerator DamageOverTimeRoutine(int damage, float lengthOfDOTEffect, float delayBetweenTicksInSeconds)
    {
        float tickCount = lengthOfDOTEffect / delayBetweenTicksInSeconds;

        for (int i = 0; i < tickCount; i++)
        {
            TakeDamage(damage);
            yield return new WaitForSeconds(delayBetweenTicksInSeconds);
        }
    }

    #endregion

    #region Currency Functions

    public void AddConfidenceCurrency(int amount)
    {
        confidenceCurrency += amount;
    }

    public void RemoveConfidenceCurrency(int amount)
    {
        confidenceCurrency -= amount;
    }

    #endregion

    #region Health Functions

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        UpdateHealthBarGUI();
    }

    void UpdateHealthBarGUI()
    {
        /*for (int i = (int)(baseHealth * 0.2f) - 1; i > 0; i--)
        {
            if (i > currentHealth * 0.2)
            {
                hearts[i].SetActive(false);
            } 
            else
            {
                hearts[i].SetActive(true);
            }
        }

        if (currentHealth == 0)
        {
            foreach (GameObject heart in hearts)
            {
                heart.SetActive(false);
            }
        }*/

        healthBarSlider.value = currentHealth;
    }

    void GetHealthBarIcons()
    {
        List<GameObject> heartContainers = new List<GameObject>();

        List<GameObject> tempHearts = new List<GameObject>();

        /*foreach (Transform child in healthBar.transform)
        {
            heartContainers.Add(child.gameObject);
        }

        foreach (GameObject _object in heartContainers)
        {
            foreach (Transform child in _object.transform)
            {
                tempHearts.Add(child.gameObject);
            }
        }

        hearts = tempHearts.ToArray();*/
    }

    public void ResetHealth()
    {
        currentHealth = baseHealth;
        UpdateHealthBarGUI();
    }
    #endregion

    #region Effect Functions

    //Stun Player Function
    public void StunPlayer(float stunDurationInSeconds, bool isInvulnerable)
    {
        StartCoroutine(StunPlayerRoutine(stunDurationInSeconds, isInvulnerable));
    }
    
    IEnumerator StunPlayerRoutine(float stunDurationInSeconds, bool _canTakeDamage)
    {
        movementStateMachine.canMove = false;
        isStunned = true;
        canTakeDamage = _canTakeDamage;
        stunParticles.Play();
        yield return new WaitForSeconds(stunDurationInSeconds);
        if (!movementStateMachine.weaponDisabledMovement) //If a weapon has disabled movement then movement should be re-enabled on weapon script
        {
            movementStateMachine.canMove = true;
        }
        isStunned = false;
        canTakeDamage = true;
        stunParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    //Slow Percentage should be between 1 - 100, this is then converted to a percentage
    public void SlowPlayer(int slowPercentage, float slowDurationInSeconds)
    {
        StartCoroutine(SlowPlayerRoutine(slowPercentage, slowDurationInSeconds));
    }


    //Slow Player Function
    IEnumerator SlowPlayerRoutine(int slowPercentage, float slowDurationInSeconds)
    {
        if (slowPercentage > 100)
        {
            slowPercentage = 100;
        }

        if (slowPercentage <= 0)
        {
            StunPlayer(slowDurationInSeconds, false);
        }
        
        movementStateMachine.agent.speed = baseSpeed * (1 - (slowPercentage * 0.01f));
        yield return new WaitForSeconds(slowDurationInSeconds);
        movementStateMachine.agent.speed = baseSpeed;

    }

    public void ResetSpeed()
    {
        StopAllCoroutines();
        RemoveStunState();
        movementStateMachine.agent.speed = baseSpeed;
    }

    public void RemoveStunState()
    {
        if (IsStunned)
        {
            movementStateMachine.canMove = true;
            isStunned = false;
            canTakeDamage = true;
            stunParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    #endregion

    void PlayerDeath()
    {
        movementStateMachine.canMove = false;
        GetComponent<Animator>().SetBool("isDead", true);

        isStunned = false;
        isSlowed = false;
        ResetSpeed();

        GameManager.gameManager.bPlayerDead = true;
        FindObjectOfType<SCR_PauseMenu>().DisableQuickAccessMenu();
        SCR_AudioManager.instance.Play("SFX_Char_Hit");
    }

    public void ShowGameOverScreen()
    {
        gameOverCanvas.SetActive(true);
        SCR_AudioManager.instance.Play("SFX_Game_Lose");
        Time.timeScale = 0;
        Cursor.visible = true;
    }
}