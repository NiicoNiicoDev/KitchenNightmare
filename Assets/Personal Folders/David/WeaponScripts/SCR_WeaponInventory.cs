using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles the current inventory for the player
public class SCR_WeaponInventory : MonoBehaviour
{
    //Contains the weapons chosen for the round
    public GameObject[] roundWeapons;

    //references the attack script
    private SCR_PlayerAttack attackScript;

    //holds bools for the weapon slots that can still be used
    [HideInInspector] public bool[] slotsEnabled = { true, true, true};

    // Start is called before the first frame update
    void Start()
    {
        //roundWeapons = new GameObject[3];

        //find the attack script in the game object
        attackScript = GetComponent<SCR_PlayerAttack>();

        Debug.Log("Array length: " + roundWeapons.Length);
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Time.timeScale != 0)
        {
            //Controls the weapon that is selected by the player
            if (Input.GetKeyDown(KeyCode.Alpha1) && slotsEnabled[0])
            {
                SetWeaponPrefab(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && slotsEnabled[1])
            {
                SetWeaponPrefab(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && slotsEnabled[2])
            {
                SetWeaponPrefab(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && slotsEnabled[3])
            {
                SetWeaponPrefab(3);
            }
        }*/
    }

    public void SetWeaponPrefab(int index)
    {
        //checks the current slot isn't empty (which it shouldn't be once the game is finished) as a fail safe to avoid a potential crash
        if (roundWeapons[index])
        {
            //set the weapon prefab to the weapon chosen by the player
            attackScript.weaponPrefab = roundWeapons[index];

            //spawns the new weapon that the player is holding (and destroys old one)
            attackScript.ChangeWeapon(index);
        }
    }

    //called when the round starts
    public void InitialiseWeaponPrefab()
    {
        //if a weapon at slot 1 exists (again, there should be as the round is programmed only to start when a weapon is assigned to each slot)
        if (roundWeapons[0])
        {
            //set the weapon prefab to the weapon at slot 1
            attackScript.weaponPrefab = roundWeapons[0];

            //change the weapon so it spawns the weapon at slot 1
            attackScript.ChangeWeapon();
        }
    }
}
