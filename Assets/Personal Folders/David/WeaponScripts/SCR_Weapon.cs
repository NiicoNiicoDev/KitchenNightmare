using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the weapon actions for each weapon
public class SCR_Weapon : MonoBehaviour
{
    #region VARIABLES
    //contains the stats for the weapon
    public SCR_WeaponStats weaponStats;

    //Current upgrade level of the weapon
    public int upgradeLevel = 1;

    //how long the attack animation lasts
    [Header(header: "how long the attack animation lasts")]
    [SerializeField] private float attackTime = 1f;

    //layer mask used to detect enemies
    [SerializeField] private LayerMask enemyLayerMask;

    //how many attacks can be done with the weapon per round
    [HideInInspector] public int attacksRemaining = 0;

    //reference to the attack script
    private SCR_PlayerAttack attackScript;

    //damage done to enemy if hit
    private float attackDamage = 0f;

    private float range = 0f;

    private float fireRate = 0f;

    //determines whether the weapon can be used an unlimited number of times per round or not
    private bool attackLimit = true;

    //stores the hit when raycasting
    private RaycastHit hit;

    //monitors how long it was since last attack to prevent button spamming
    private float timeSinceLastAttack = 0f;
    #endregion

    #region START
    // Start is called before the first frame update
    void Start()
    {
        #region TEXTURE SETUP
        if (upgradeLevel <= weaponStats.weaponTextures.Length)
        {
            GetComponent<Renderer>().material = weaponStats.weaponTextures[upgradeLevel - 1];
        }
        #endregion

        #region UPGRADE SETUP
        fireRate = weaponStats.attackRate;
        attackDamage= weaponStats.attackDamage;
        range = weaponStats.maximumRange;

        foreach(SCR_WeaponStats.UpgradeTraits trait in weaponStats.upgradeTraits)
        {
            switch(trait)
            {
                case SCR_WeaponStats.UpgradeTraits.RANGE:
                    range *= upgradeLevel;
                    break;
                case SCR_WeaponStats.UpgradeTraits.DAMAGE:
                    //Set the attack damage to the default damage of the weapon and multiply by the upgrade level
                    attackDamage *= upgradeLevel;
                    break;
                case SCR_WeaponStats.UpgradeTraits.FIRERATE:
                    fireRate/= upgradeLevel;
                    break;
                default:
                    break;

            }
        }

        Debug.Log("attack damage: " + attackDamage + "\nrange: " + range + "\nfire rate: " + fireRate);
        #endregion

        //allows the user to attack once the scene is loaded
        timeSinceLastAttack = weaponStats.attackRate;

        ResetAttackLimit();

        //initialise the attack script. The weapon is a child of the player, so GetComponentInParent<> will find the correct attack script
        attackScript = GetComponentInParent<SCR_PlayerAttack>();
    }
    #endregion

    #region UPDATE
    // Update is called once per frame
    void Update()
    {
        //if there is an attack limit and the weapon has be used too many times
        if(attacksRemaining <= 0 && attackLimit)
        {
            //disable the weapon slot as the attack limit has been reached
            attackScript.DisableSlot();

            //destroy the weapon so it can no longer be used
            Destroy(gameObject);
        }

        //increase the timer
        timeSinceLastAttack += Time.deltaTime;
    }
    #endregion

    #region ATTACK FUNCTIONS
    //called when player dies to reset the attack limit
    public void ResetAttackLimit()
    {
        //if maximum attacks is set to more than 0
        if (weaponStats.maximumAttacks > 0)
        {
            //Set the attack remaining to the maximum amounts
            attacksRemaining = weaponStats.maximumAttacks;
        }
        else
        {
            //else the weapon can be used an unlimited number of times
            attackLimit = false;
        }
    }
    
    //public function which is called when attacking
    public void Attack()
    {
        if (timeSinceLastAttack >= weaponStats.attackRate)
        {
            Debug.Log("attack");

            //check if weapon is a projectile or an AOE
            if (weaponStats.attackType == SCR_WeaponStats.AttackTypes.PROJECTILE)
            {
                Debug.DrawRay(transform.position, transform.forward * weaponStats.maximumRange, Color.red);
                //If projectile, create a raycast to the max range of the weapon
                if (Physics.Raycast(transform.position, transform.forward, out hit, weaponStats.maximumRange))
                {
                    //if an enemy is hit
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        //damage the enemy
                        hit.collider.GetComponent<SCR_EnemyHealth>().TakeDamage((int)attackDamage);
                    }
                }
            }
            else
            {
                //else, it is an AOE so find enemy colliders within the range of the weapon
                //TODO: allow for offsets with some weapons, such as the blender
                Collider[] enemyColliders = Physics.OverlapSphere(transform.position, weaponStats.maximumRange, enemyLayerMask);

                //then get each enemy collider found
                foreach (Collider enemyCollider in enemyColliders)
                {
                    //and call the damage function in the health script
                    enemyCollider.GetComponent<SCR_EnemyHealth>().TakeDamage((int)attackDamage);

                    Debug.Log("Hit Enemy");
                }
            }
            
            //reset the timer so another attack cannot be made until allowed
            timeSinceLastAttack = 0f;

            //if there is an attack limit
            if (attackLimit)
            {
                //reduce the attacks remaining count
                attacksRemaining--;
            }
        }
    } //TODO: Change enemy damage script to one used in final game
    #endregion
    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIT");
        //if the collision is with an enemy
        if (other.CompareTag("Enemy"))
        {
            //do damage to that enemy
            //TODO: Damage enemy via completed enemy health script
            other.GetComponent<TEMP_EnemyHealth>().Damage(attackDamage);
        }
    }*/
}
