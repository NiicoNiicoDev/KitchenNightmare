using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//Handles the pre-round UI for setting up the player's inventory for the round
public class SCR_AddWeaponsToInventory : MonoBehaviour
{
    //TODO: change this so available weapons set by game manager based on player progress
    //Contains all currently available weapons
    [SerializeField] private GameObject[] availableWeapons;

    [SerializeField] private GameObject[] weaponButtons;

    //Contains the selection of weapons screen
    [SerializeField] private GameObject weaponUI;

    //Contains the weapon selection screen which displays before each round
    [SerializeField] private GameObject weaponSelectionUI;

    //Contains the HUD for the player
    [SerializeField] private GameObject HUD;

    [SerializeField] private GameObject weaponDescription;

    [SerializeField] private TextMeshProUGUI descriptionText;

    //reference to the weapon inventory script
    // REDUNDANT private SCR_WeaponInventory weaponInventory;
    private SCR_WeaponHandler weaponHandler;

    //contains the index of the weapon slot to be changed
    private int weaponIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Initialise weapon inventory script.
        //Make sure the player's name is used in the find slot, else it will likely trigger a crash when this script is called.
        weaponHandler = FindObjectOfType<SCR_WeaponHandler>();

        //initialise weapon UI screens so that the selection screen displays before each round begins
        weaponUI.SetActive(true);
        weaponSelectionUI.SetActive(true);
        HUD.SetActive(false);
        weaponDescription.SetActive(false);

        availableWeapons = GameManager.gameManager.unlockedWeapons.ToArray();

        //pause the game by setting time scale to zero
        Time.timeScale = 0;

        //this prevents the game from crashing if the UI hasn't been set up
        if (weaponButtons.Length > 0)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                //if the index is out of bounds
                if (i >= availableWeapons.Length)
                {
                    //button isn't needed as all weapons are displayed, so hide it
                    weaponButtons[i].SetActive(false);
                    continue;
                }

                //set the button text to the weapon name
                weaponButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = availableWeapons[i].name;
            }
        }
    }

    //called by UI buttons to add items to the inventory
    public void AddWeapon(int index)
    {
        if (index >= availableWeapons.Length)
        {
            return;
        }

        foreach (GameObject weapon in weaponHandler.EquippedWeapons)
        {
            if(weapon == availableWeapons[index])
            {
                return;
            }
        }

        //set the round weapon at the given slot to the weapon selected
        //weaponInventory.roundWeapons[weaponIndex] = availableWeapons[index];
        weaponHandler.AddWeaponToEquippedWeapons(weaponIndex, availableWeapons[index]);
        

        Debug.Log("Added " + availableWeapons[index].name + " to inventory");

        //Close the weapon screen (ignore the 0, used so method can be accessed as required for when opening weapon screen)
        ToggleWeaponScreen(0);

        weaponDescription.SetActive(false);

        //TODO: Display an icon on the button of the chosen weapon

        //TODO: add a checker to see if the weapon is unlocked
    }

    //opens or closes the weapon selection. Also sets the index of the weapon slot.
    public void ToggleWeaponScreen(int index)
    {
        //if active then hide screen and vice versa
        if (weaponUI.activeSelf)
        {
            weaponUI.SetActive(false);
        }
        else
        {
            weaponUI.SetActive(true);

            weaponIndex = index;
            //TODO: set up weapon UI screen
        }
    }

    //begins the round if 3 weapons have been selected
    public void StartRound()
    {
        //searches the weapon array in inventory script
        foreach(GameObject weapon in weaponHandler.EquippedWeapons)
        {
            //if an item is null
            if (!weapon)
            {
                Debug.Log("Not enough weapons selected");
                //leave the method as the round shouldn't start
                return;
            }
        }

        //close the UI and display the HUD
        weaponUI.SetActive(false);
        weaponSelectionUI.SetActive(false);
        HUD.SetActive(true);

        //set the time scale to 1 to un-pause the game (and start the round)
        Time.timeScale = 1;

        //spawn the weapon at slot 1 in the player's inventory
        
        //Deal with this later ~GEORGE
        //weaponInventory.InitialiseWeaponPrefab();
    }

    public void DisplayDescription(int index)
    {
        if (index >= availableWeapons.Length)
        {
            return;
        }

        weaponDescription.SetActive(true);
        descriptionText.text = availableWeapons[index].GetComponent<SCR_Weapon>().weaponStats.weaponDescription;
    }

    public void HideDescription()
    {
        weaponDescription.SetActive(false);
    }
}
