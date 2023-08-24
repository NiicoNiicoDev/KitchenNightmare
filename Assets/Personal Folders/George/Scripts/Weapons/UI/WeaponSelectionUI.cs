using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionUI : MonoBehaviour
{
    int targetSlot = 0;

    public GameObject basicSelectionUI;
    public GameObject abilitySelectionUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTargetSlot(int target)
    {
        targetSlot = target;
    }

    public void EnableBasicSelectionUI()
    {
        abilitySelectionUI.SetActive(false);
        basicSelectionUI.SetActive(true);
    }

    public void EnableAbilitySelectionUI()
    {
        basicSelectionUI.SetActive(false);
        abilitySelectionUI.SetActive(true);
    }

    public void SetSelectedWeapon(int targetWeaponIndex)
    {
        SCR_WeaponHandler weaponHandler = FindObjectOfType<SCR_WeaponHandler>();
        
        if (weaponHandler.Weapons[targetWeaponIndex].GetComponent<SCR_BaseWeapon>().IsUnlocked == false)
        {
            return;
        } 
        else
        {
            weaponHandler.UpdateEquippedWeapons(targetSlot, targetWeaponIndex);
            FindObjectOfType<SCR_WeaponUI>().UpdateWeaponUI(targetSlot);
        }
    }

    public void PlayInteractSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }
}
