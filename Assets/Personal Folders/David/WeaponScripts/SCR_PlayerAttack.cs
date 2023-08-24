using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the behaviour of the player when attacking
public class SCR_PlayerAttack : MonoBehaviour
{
    //contains the prefab of the chosen weapon
    //TODO: allow this to be selected via UI and not in inspector
    public GameObject weaponPrefab;

    //holds information about whether the player is attacking for the hazards
    [HideInInspector] public bool bAttacking = false;

    //where the weapon should be spawned
    [SerializeField] private Transform weaponSpawnPoint;

    //feedback to the user that the player is attacking (temporary while no animation)
    [SerializeField] private GameObject attackText;

    [SerializeField] private Animator playerAnimator;

    //holds the spawned weapon so weapon script can be accessed
    private GameObject chosenWeapon;

    //reference to the inventory script
    private SCR_WeaponInventory inventory;

    //holds details about which slots have already been used by the player
    private bool[] slotsUsedPreviously = { false, false, false};

    //contains the weapon script
    private SCR_Weapon weapon;

    //the current weapon slot in use by the player
    private int currentSlot = 0;

    //holds details about the attacks left for each weapon slot after use
    private int[] remainingAttacks = { 0, 0, 0, 0 };

    private void Start()
    {
        //initialise inventory script
        inventory = GetComponent<SCR_WeaponInventory>();

        attackText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            //TODO: Set input to right mouse button and include gamepad controls //<--- use in Scene_David
            //if attack button is pressed
            if (Input.GetKeyDown(KeyCode.K) && weapon)
            {
                //set the attacking bool to true
                bAttacking = true;

                //attack using the current weapon
                weapon.Attack();

                playerAnimator.SetTrigger(weapon.weaponStats.attackTriggerName);

                attackText.SetActive(true);
            }
            else
            {
                //set the attacking bool to false
                bAttacking = false;

                attackText.SetActive(false);
            }
        }
    }

    /*public IEnumerator UseWeapon() //<--- use in game scenes
    {
        yield return null;

        Debug.Log("Use Weapon");
        //set the attacking bool to true
        bAttacking = true;

        //attack using the current weapon
        weapon.Attack();

        playerAnimator.SetTrigger(weapon.weaponStats.attackTriggerName);

        yield return null;

        bAttacking = false;

        //attackText.SetActive(true);
    }*/

    //called after the player dies so the weapon attack counts reset
    public void ResetWeapons()
    {
        //find each weapon and reset the attack limits
        foreach(GameObject weapon in inventory.roundWeapons)
        {
            weapon.GetComponent<SCR_Weapon>().ResetAttackLimit();
        }

        //set each slot to not previously used
        for (int i = 0; i < slotsUsedPreviously.Length; i++)
        {
            slotsUsedPreviously[i] = false;
        }

        //enable every slot again
        for (int i = 0; i < inventory.slotsEnabled.Length; i++)
        {
            inventory.slotsEnabled[i] = true;
        }

        //respawn the weapon the player was holding before dying (not necessarily the weapon at slot 1)
        ChangeWeapon(currentSlot);
    }

    public void ChangeWeapon()
    {
        //spawn the chosen weapon
        chosenWeapon = (GameObject)Instantiate(weaponPrefab, weaponSpawnPoint.position, weaponSpawnPoint.rotation);

        //set it as a child of the player game object
        chosenWeapon.transform.parent = transform;

        //Set up weapon script
        StartCoroutine("SetWeaponScript");
        //TODO: adjust this so selected weapon is spawned and weapon script from that weapon is used
    }

    //prevents player from using a weapon that has exceeded its use limit
    public void DisableSlot()
    {
        //disable the slot
        inventory.slotsEnabled[currentSlot] = false;

        //then set the weapon to null so the script isn't called in the Update() function (which would trigger a crash)
        weapon = null;
    }

    public void ChangeWeapon(int slot)
    {
        //if a weapon exists (i.e., it has not already exceeded its maximum use limit)
        /*if (weapon)
        {
            //store the remaining attacks left so it is remembered when the weapon is next used
            remainingAttacks[currentSlot] = weapon.attacksRemaining;
        }*/

        //reset the weapon script
        weapon = null;

        //destroy the current weapon to avoid multiple weapons stacking on each other
        Destroy(chosenWeapon);

        //spawn the chosen weapon
        chosenWeapon = (GameObject)Instantiate(weaponPrefab, weaponSpawnPoint.position, weaponSpawnPoint.rotation);

        //set it as a child of the player game object
        chosenWeapon.transform.parent = weaponSpawnPoint.transform;

        //change the weapon slot in use
        currentSlot = slot;

        //Set up weapon script
        StartCoroutine("SetWeaponScript");
        //TODO: adjust this so selected weapon is spawned and weapon script from that weapon is used
    }

    IEnumerator SetWeaponScript()
    {
        //triggers a delay so weapon is set after original weapon is destroyed
        yield return null;

        //initalises weapon script.
        weapon = GetComponentInChildren<SCR_Weapon>();

        //if the slot has been used before then adjust the remaining attacks to the correct amount
        /*if (slotsUsedPreviously[currentSlot])
        {
            weapon.attacksRemaining = remainingAttacks[currentSlot];
        }
        else
        {
            //otherwise, set the slot to a used slot for when it is next called
            slotsUsedPreviously[currentSlot] = true;
        }*/
    }
}
