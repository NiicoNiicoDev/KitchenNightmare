using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

//contains the shop button functions
public class SCR_StoreUIHandler : MonoBehaviour
{
    [Header(header:"See script comments for variable descriptions")]
    //name of the next level
    [SerializeField] private string nextScene = "SN_Level01";

    //contains the weapons found in the store
    [SerializeField] private List<GameObject> storeWeapons;

    //contains the weapons which can be upgraded in this store (Note: doesn't neccessarily refer to every weapon which can be upgraded)
    [SerializeField] private List<GameObject> upgradableWeapons;

    //reference to the buttons used to buy weapons. Should be in order of how they are listed in the UI.
    [SerializeField] private GameObject[] weaponButtons;

    //reference to the buttons used to upgrade weapons
    [SerializeField] private GameObject[] upgradeButtons;

    //reference to the selection of weapons screen in the shop UI
    [SerializeField] private GameObject newWeaponSelection;

    //reference to the selection of weapon upgrades available screen in the shop UI
    [SerializeField] private GameObject weaponUpgradeSelection;

    [SerializeField] private GameObject weaponDescription;

    [SerializeField] private TextMeshProUGUI descriptionText;

    private int costIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //iterate through the provided weapons in the store
        for (int i = 0; i < storeWeapons.Count; i++)
        {
            //if the weapon is already unlocked
            if (GameManager.gameManager.unlockedWeapons.Contains(storeWeapons[i]))
            {
                //remove the weapon so it can't be added multiple times
                storeWeapons.Remove(storeWeapons[i]);

                i--;
            }
        }

        //iterate through the upgradable weapons list
        /*for(int i = 0; i < upgradableWeapons.Count; i++) Adapt if weapon upgrading is implemented
        {
            //check for weapons listed already at their highest upgrade level and weapons that haven't yet been unlocked
            if(upgradableWeapons[i].GetComponent<SCR_Weapon>().upgradeLevel > upgradableWeapons[i].GetComponent<SCR_Weapon>().weaponStats.upgradeCosts.Length || !GameManager.gameManager.unlockedWeapons.Contains(upgradableWeapons[i]))
            {
                //remove these weapons as they shouldn't be upgraded
                upgradableWeapons.Remove(upgradableWeapons[i]);

                i--;
            }
        }*/

        /*//weapons that have not yet been bought should be at level 1
        foreach(GameObject weapon in storeWeapons)
        {
            weapon.GetComponent<SCR_Weapon>().upgradeLevel = 1;
        }*/

        //set up the weapon and upgrade buttons
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            Debug.Log("Weapons: " + storeWeapons.Count);
            if (i >= storeWeapons.Count)
            {
                //hide the button as this weapon/weapon upgrade shouldn't be purchased as the player already has it
                weaponButtons[i].SetActive(false);
            }
            else
            {
                Debug.Log("index: " + i);
                //create a temporary cost variable to hold the cost
                int cost;

                //cost = storeWeapons[i].GetComponent<SCR_BaseWeapon>().cost;

                weaponButtons[i].GetComponent<Image>().sprite = storeWeapons[i].GetComponent<SCR_BaseWeapon>().weaponSprite;

                //SetUpButtons(upgradeButtons, upgradableWeapons, true);

                //descriptionText.text = "Cost: " + cost + " confidence";

                weaponDescription.SetActive(false);
            }
        }
    }

    //this function is used to make the Start() function tidier and sets up the relevant buttons
    /*void SetUpButtons(GameObject[] buttons, List<GameObject> items, bool upgrade)
    {
        //iterate through the button array
        
                *//*else
                {
                    //variables used to make text setup more readable. Get the upgrade level and the upgrade cost.
                    int newUpgradeLevel = items[i].GetComponent<SCR_Weapon>().upgradeLevel + 1;
                    costIndex = items[i].GetComponent<SCR_Weapon>().upgradeLevel - 1;
                    cost = items[i].GetComponent<SCR_Weapon>().weaponStats.upgradeCosts[costIndex];
                    string name = items[i].GetComponent<SCR_Weapon>().weaponStats.name;

                    //set up the button text. Include the name, upgrade level and its cost in confidence
                    buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = name + " Level " + newUpgradeLevel + " (" + cost + " confidence)";
                }*//*
            }
        }
    }*/

    //called by the close shop button to close the shop
    public void CloseShop()
    {
        GameManager.gameManager.LoadNextRound(nextScene);
    }

    //adds a new weapon that the player can use
    public void UnlockWeapon(int weaponIndex)
    {
        weaponDescription.SetActive(false);

        //adds the new weapon to the list of unlocked weapons
        GameManager.gameManager.unlockedWeapons.Add(storeWeapons[weaponIndex]);

        //decreases the confidence based on the weapon cost
        //GameManager.gameManager.DecreaseConfidence(storeWeapons[weaponIndex].GetComponent<SCR_BaseWeapon>().cost);

        //disable the button as weapon has been bought (and buying it again would create duplicates)
        weaponButtons[weaponIndex].GetComponent<Button>().interactable= false;

        //check all the buttons to disable any that the player can no longer afford
        CheckButtons();

        Debug.Log("You bought a " + storeWeapons[weaponIndex].name);
    }

    //scans the buttons to see if the player can afford the weapon/weapon upgrade
    public void CheckButtons()
    {
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (weaponButtons[i].activeSelf)
            {
                /*if (storeWeapons[i].GetComponent<SCR_BaseWeapon>().cost > GameManager.gameManager.currentConfidence)
                {
                    //if the player cannot afford the weapon (i.e., confidence is less than weapon value), disable the button so the item cannot be bought
                    weaponButtons[i].GetComponent<Button>().interactable = false;
                }*/
            }
        }

        /*for(int i = 0; i < upgradeButtons.Length; i++)
        {
            if (upgradeButtons[i].activeSelf && upgradeButtons[i].GetComponent<Button>().interactable)
            {
                costIndex = upgradableWeapons[i].GetComponent<SCR_Weapon>().upgradeLevel - 1;
                if (upgradableWeapons[i].GetComponent<SCR_Weapon>().weaponStats.upgradeCosts[costIndex] > GameManager.gameManager.currentConfidence)
                {
                    //if the player cannot afford the upgrade, disable the button
                    upgradeButtons[i].GetComponent<Button>().interactable = false;
                }
            }
        }*/
    }
    
    //called by weapon button to display the available weapons (and hide the upgrade selection)
    public void OpenWeaponSelection()
    {
        newWeaponSelection.SetActive(true);
        weaponUpgradeSelection.SetActive(false);
    }

    /*//called by the upgrade button to display the available weapon upgrades (and hide the weapon selection)
    public void OpenUpgradeSelection()
    {
        newWeaponSelection.SetActive(false);
        weaponUpgradeSelection.SetActive(true);
    }*/

    //called by the respective upgrade button to upgrade a weapon
   /* public void UpgradeWeapon(int index)
    {
        //this gets the position of the weapon to be upgraded in the game manager's unlockable weapons list
        int upgradeIndex = GameManager.gameManager.unlockedWeapons.IndexOf(upgradableWeapons[index]);

        //this checks that the weapon was found in the unlocked weapons list (if it wasn't, it should have returned -1)
        if (upgradeIndex > -1)
        {
            Debug.Log("Upgraded " + upgradableWeapons[index].name);

            costIndex = upgradableWeapons[index].GetComponent<SCR_Weapon>().upgradeLevel - 1;

            //increase the upgrade level of the respective weapon
            GameManager.gameManager.unlockedWeapons[upgradeIndex].GetComponent<SCR_Weapon>().upgradeLevel++;

            GameManager.gameManager.DecreaseConfidence(upgradableWeapons[index].GetComponent<SCR_Weapon>().weaponStats.upgradeCosts[costIndex]);

            //disable the button so the player can't keep upgrading the weapons
            //for now, this means each weapon can only be upgraded ONCE per round at most
            upgradeButtons[index].GetComponent<Button>().interactable = false;

            CheckButtons();
        }
    }*/

    public void DisplayDescription(int index)
    {
        if (index >= storeWeapons.Count)
        {
            return;
        }

        weaponDescription.SetActive(true);
        descriptionText.text = storeWeapons[index].GetComponent<SCR_Weapon>().weaponStats.weaponDescription;
    }

    public void HideDescription()
    {
        weaponDescription.SetActive(false);
    }
}
