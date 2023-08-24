using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SCR_BaseWeapon : MonoBehaviour
{
    [Header("Damage Variables")]
    [SerializeField] protected int baseDamage;
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;

    public float Cooldown { get { return cooldown; } }

    public bool IsUnlocked { get { return isUnlocked; } }
    
    public int CurrentUpgradeLevel { get { return currentUpgradeLevel; } }
    public int[] UpgradeCosts { get { return upgradeCosts; } }


    [Header("Visual Effects")]
    [SerializeField] protected string animName;
    [SerializeField] protected GameObject VFX_Prefab;
    [SerializeField] protected GameObject attackOutlinePrefab;
    protected GameObject VFX_Object;
    protected GameObject attackOutlineObject;
    [Tooltip("If left empty will default to player position")]
    [SerializeField] protected Transform VFX_Spawn_Point;
    [SerializeField] protected Vector3 VFX_Spawn_Rotation;

    [SerializeField] private bool attatchAnimToPlayer = true;
    [Tooltip("Make sure attatch to player is turned off!")]
    [SerializeField] private bool attatchAnimToWeapon = false;
    [SerializeField] private bool killAnimOnAttackEnd = true;
    [SerializeField] private bool shouldRotate = true;
    [SerializeField] private bool canPlayerMove = true;

    [Header("Audio Effects")]
    [SerializeField] private AudioClip equipSound; 
    [SerializeField] private AudioClip soundToPlay;
    [SerializeField] private bool playUntilFinish = true;
    [SerializeField] private bool shouldLoop = false;
    private AudioSource audioSource;
    private AudioMixerGroup audioMixer;

    [Header("Upgrade Stats")]
    [SerializeField] protected bool isUnlocked;
    [SerializeField] protected int currentUpgradeLevel;
    [SerializeField] protected int[] upgradeCosts;
    
    [Header("UI References")]
    [SerializeField] public Sprite weaponSprite;

    [Header("Other References")]
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected Transform weaponTransform;

    protected virtual void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        weaponTransform = transform;

        audioSource = playerTransform.GetComponent<AudioSource>();
        try
        {
            audioMixer = FindObjectOfType<SCR_AudioManager>().sfxMixerGroup;
            audioSource.outputAudioMixerGroup = audioMixer;
        }
        catch (System.NullReferenceException)
        {
            Debug.LogWarning("SCR_AudioManager Required In Scene (Get Szymon's Prefab)"); //Throwing an error here stops some weapons from initializing properly (Therefore its a warning instead)
        }
    }

    private void Update()
    {
       
    }

    protected virtual void FixedUpdate()
    {
        if (!shouldRotate)
        {
            weaponTransform.rotation = Quaternion.identity;
        }
    }

    public virtual void Attack() 
    {
        SetWeaponActive();
        /*remainingCooldown = cooldown;
        isOnCooldown = true;*/
    }
    
    public virtual void UpgradeWeapon(int newLevel)
    {

    }

    public void PlaySound()
    {
        audioSource.loop = shouldLoop;
        audioSource.clip = soundToPlay;
        audioSource.Play();
    }

    
    public virtual void PlayVFX()
    {
        Transform parent = null;
        if (attatchAnimToPlayer)
        {
            parent = playerTransform;
        }
        else if(attatchAnimToWeapon)
        {
            parent = weaponTransform;
        }

        Vector3 spawnPos = playerTransform.position;
        if (VFX_Spawn_Point)
        {
            spawnPos = VFX_Spawn_Point.position;
        }

        if (VFX_Prefab == null) return;

        VFX_Object = Instantiate(VFX_Prefab, spawnPos, Quaternion.identity, parent);
        VFX_Object.transform.localRotation = Quaternion.Euler(VFX_Spawn_Rotation);
    }

    public virtual void PlayAttackAnimation()
    {
        Animator animator = playerTransform.gameObject.GetComponent<Animator>();

        if (animName == null) return;

        if (animator.GetBool("isDead") == true)
        {
            return;
        }

        animator.Play(animName);
    }

    public virtual void SetWeaponActive()
    {
        Destroy(attackOutlineObject);

        gameObject.SetActive(true);

        if (!canPlayerMove)
        {
            playerTransform.GetComponent<PSM_MovementStateMachine>().SetWeaponDisabledMovement(true);
        }

        if (equipSound != null)
        {
            audioSource.clip = equipSound;
            audioSource.Play();
        }
    }

    public virtual void SetWeaponInactive()
    {
        if (attackOutlineObject != null)
            Destroy(attackOutlineObject);

        gameObject.SetActive(false);

        if (killAnimOnAttackEnd)
        {
            Destroy(VFX_Object);
        }

        if (!canPlayerMove)
        {
            playerTransform.GetComponent<PSM_MovementStateMachine>().SetWeaponDisabledMovement(false);
        }

        if (!playUntilFinish || shouldLoop)
        {
            audioSource.Stop();
        }

        FindObjectOfType<PSM_InputHandler>().IsAttacking = false;
    }

    public virtual void OnAttackEnded()
    {

    }

    public virtual void EnableCustomMesh()
    {

    }

    public void UnlockWeapon()
    {
        isUnlocked = true;
    }

    public void UpgradeWeapon()
    {
        if (currentUpgradeLevel < upgradeCosts.Length)
        {
            FindObjectOfType<SCR_PlayerStats>().RemoveConfidenceCurrency(upgradeCosts[currentUpgradeLevel]);

            currentUpgradeLevel += 1;
            damage = (int)(baseDamage * (0.75f * currentUpgradeLevel));
        }
    }

    public virtual void DrawAttackArea()
    {
        if (attackOutlinePrefab != null)
        {
            attackOutlineObject = Instantiate(attackOutlinePrefab, playerTransform.position, playerTransform.rotation * Quaternion.Euler(0, 180, 0), playerTransform);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerTransform.position, 0.5f);
    }
}
