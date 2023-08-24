using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_WeaponUI : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private GameObject[] weaponSlots = new GameObject[4];
    [SerializeField] private SCR_BaseWeapon[] weaponScripts = new SCR_BaseWeapon[4];
    [SerializeField] private Image[] weaponSlotCooldown = new Image[4];
    
    private SCR_WeaponHandler weaponHandler;

    private void Awake()
    {
        weaponHandler = FindObjectOfType<SCR_WeaponHandler>();

        /*weaponSlots[0].GetComponent<Image>().sprite = FindObjectOfType<Weapon_Spoon>().weaponSprite;*/

        if (weaponHandler.EquippedWeapons.Length == 4)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                try
                {
                    weaponScripts[i] = weaponHandler.EquippedWeapons[i].GetComponent<SCR_BaseWeapon>();
                    weaponSlots[i].GetComponent<Image>().sprite = weaponHandler.EquippedWeapons[i].GetComponent<SCR_BaseWeapon>().weaponSprite;
                    weaponSlotCooldown[i] = weaponSlots[i].transform.GetChild(0).GetComponent<Image>();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e, this);
                }

            }
        }

        /*for (int i = 0; i < weaponHandler.EquippedWeapons.Length; i++)
        {
            try
            {
                weaponScripts[i] = weaponHandler.EquippedWeapons[i].GetComponent<SCR_BaseWeapon>();
                weaponSlots[i].GetComponent<Image>().sprite = weaponHandler.EquippedWeapons[i].GetComponent<SCR_BaseWeapon>().weaponSprite;
                weaponSlotCooldown[i] = weaponSlots[i].transform.GetChild(0).GetComponent<Image>();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e, this);
            }

        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < weaponHandler.EquippedWeapons.Length; i++)
        {      
            if (weaponHandler.EquippedWeapons[i] != null && weaponHandler.BWeaponOnCooldown[i])
            {
                UpdateCooldownOverlay(i);
            }
        }
    }

    public void UpdateCooldownOverlay(int index)
    {
        if (weaponHandler.WeaponCurrentCooldowns[index] >= 0)
            weaponSlotCooldown[index].fillAmount = weaponHandler.WeaponCurrentCooldowns[index] / weaponHandler.WeaponCooldowns[index];
    }
    
    public void UpdateWeaponUI(int index)
    {
        weaponScripts[index] = weaponHandler.EquippedWeapons[index].GetComponent<SCR_BaseWeapon>();
        weaponSlots[index].GetComponent<Image>().sprite = weaponHandler.EquippedWeapons[index].GetComponent<SCR_BaseWeapon>().weaponSprite;
        weaponSlotCooldown[index] = weaponSlots[index].transform.GetChild(0).GetComponent<Image>();
    }
}
