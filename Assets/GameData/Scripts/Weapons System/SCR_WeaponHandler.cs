using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_WeaponHandler : MonoBehaviour
{
    bool bIsTesting = false;
    
    [Header("Weapon References")]
    [SerializeField] private GameObject[] weapons = new GameObject[8];
    [SerializeField] private GameObject basicWeapon = null;
    [SerializeField] private GameObject[] equippedWeapons = new GameObject[4];

    [Space(5)]

    [SerializeField] private float[] weaponCooldowns = new float[4];
    [SerializeField] private float[] weaponCurrentCooldowns = new float[4];
    [SerializeField] private bool[] bWeaponOnCooldown = new bool[4];

    public GameObject[] Weapons { get { return weapons; } }
    public float[] WeaponCooldowns { get { return weaponCooldowns; } }
    public float[] WeaponCurrentCooldowns { get { return weaponCurrentCooldowns; } }
    public bool[] BWeaponOnCooldown { get { return bWeaponOnCooldown; } }
    
    [Space(5)]
    
    private SCR_BaseWeapon currentWeapon;

    [Space(15)]

    [Header("Player References")]
    [SerializeField] private Transform weaponSpawnTransformRight;
    [SerializeField] private Transform weaponSpawnTransformLeft;

    private bool bWeaponsSpawned = false;

    private class WeaponData
    {
        private string weaponName;
        SCR_BaseWeapon weaponScript;

        private Sprite weaponSprite;
    }

    public SCR_BaseWeapon CurrentWeapon
    {
        get { return currentWeapon; }
    }
    public GameObject[] EquippedWeapons
    {
        get { return equippedWeapons; }
    }

    public void AddWeaponToEquippedWeapons(int slot, GameObject weapon)
    {
        equippedWeapons[slot] = weapon;
    }

    private void Awake()
    {
        if (equippedWeapons.Length == 4 && !bWeaponsSpawned)
        {
            SpawnWeapons();
        }

        /*if (!bWeaponsSpawned)
        {
            SpawnWeapons();
        }*/
    }

    void Update()
    {
        
    }

    public void UpdateEquippedWeapons(int index, int targetWeaponIndex)
    {
        foreach (var item in equippedWeapons)
        {
            if (item == null)
                continue;
            
            if (item.GetComponent<SCR_BaseWeapon>().GetType() == weapons[targetWeaponIndex].GetComponent<SCR_BaseWeapon>().GetType())
            {
                Debug.Log(item.GetComponent<SCR_BaseWeapon>().GetType());
                Debug.Log(weapons[targetWeaponIndex].GetComponent<SCR_BaseWeapon>().GetType());
                Debug.Log("You already have that weapon equipped");
                return;
            } 
        }

        Destroy(equippedWeapons[index]);
        equippedWeapons[index] = weapons[targetWeaponIndex];

        if (equippedWeapons[index].TryGetComponent(out Weapon_Knife knife) == true)
        {
            equippedWeapons[index] = Instantiate(equippedWeapons[index], weaponSpawnTransformLeft.position, weaponSpawnTransformLeft.rotation, weaponSpawnTransformLeft);
        }
        else
        {
            equippedWeapons[index] = Instantiate(equippedWeapons[index], weaponSpawnTransformRight.position, weaponSpawnTransformRight.rotation, weaponSpawnTransformRight);
        }

        weaponCooldowns[index] = equippedWeapons[index].GetComponent<SCR_BaseWeapon>().Cooldown;
        equippedWeapons[index].SetActive(false);
    }

    void SpawnWeapons()
    {
        //basicWeapon = Instantiate(basicWeapon, weaponSpawnTransformRight.position, weaponSpawnTransformRight.rotation, weaponSpawnTransformRight);
        
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            if (equippedWeapons[i] == null)
            {
                i++;
            }

            if (equippedWeapons[i].TryGetComponent(out Weapon_Knife knife) == true)
            {
                equippedWeapons[i] = Instantiate(equippedWeapons[i], weaponSpawnTransformLeft.position, weaponSpawnTransformLeft.rotation, weaponSpawnTransformLeft);
            } 
            else
            {
                equippedWeapons[i] = Instantiate(equippedWeapons[i], weaponSpawnTransformRight.position, weaponSpawnTransformRight.rotation, weaponSpawnTransformRight);
            }

            weaponCooldowns[i] = equippedWeapons[i].GetComponent<SCR_BaseWeapon>().Cooldown;

            //basicWeapon.SetActive(false);
            equippedWeapons[i].SetActive(false);
        }

        bWeaponsSpawned = true;
    }

    public void UpdateWeaponIndex(int index)
    {
        currentWeapon = equippedWeapons[index].GetComponent<SCR_BaseWeapon>();
    }

    public void CallCooldownRoutine(int index)
    {
        StartCoroutine(CooldownRoutine(index, weaponCooldowns[index])); 
    }

    IEnumerator CooldownRoutine(int index, float newCooldown)
    {
        float timer = newCooldown;

        bWeaponOnCooldown[index] = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            weaponCurrentCooldowns[index] = timer;
            yield return null;
        }

        if (timer <= 0)
        {
            bWeaponOnCooldown[index] = false;
            weaponCurrentCooldowns[index] = 0;

        }
    }
        

    /*if (remainingCooldown > 0 && isOnCooldown)
        {
            remainingCooldown -= Time.deltaTime;
        }

if (remainingCooldown <= 0 && isOnCooldown)
{
    isOnCooldown = false;
}*/

    public void ResetWeaponCooldowns()
    {
        StopAllCoroutines();
        SCR_WeaponUI weaponUI = FindObjectOfType<SCR_WeaponUI>();
        for (int i = 0; i < weaponCurrentCooldowns.Length; i++)
        {
            weaponCurrentCooldowns[i] = 0;
            weaponUI.UpdateCooldownOverlay(i);
            bWeaponOnCooldown[i] = false;
        }

        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            equippedWeapons[i].SetActive(false);
        }
    }

    public void ReduceWeaponCooldowns(int amountToReduce)
    {
        SCR_WeaponUI weaponUI = FindObjectOfType<SCR_WeaponUI>();
        StopAllCoroutines();
        for (int i = 0; i < weaponCurrentCooldowns.Length; i++)
        {
            if (weaponCurrentCooldowns[i] > 0)
            {
                float updatedCooldown = weaponCurrentCooldowns[i] - amountToReduce;
                if (updatedCooldown <= 0)
                {
                    weaponCurrentCooldowns[i] = 0;
                    weaponUI.UpdateCooldownOverlay(i);
                    bWeaponOnCooldown[i] = false;
                }
                else
                {
                    StartCoroutine(CooldownRoutine(i, updatedCooldown));
                }
            }
        }
    }

    #region Proxy Function Calls
    void CallAttackFunction()
    {
        currentWeapon.Attack();
    }

    void CallSetInactiveFunction()
    {
        currentWeapon.SetWeaponInactive();
    }

    void CallOnAttackEndedFunction()
    {
        currentWeapon.OnAttackEnded();
    }

    void CallEnableCustomMesh()
    {
        currentWeapon.EnableCustomMesh();
    }

    void CallActivateVFXFunction()
    {
        currentWeapon.PlayVFX();
    }

    void CallPlaySoundFunction()
    {
        currentWeapon.PlaySound();
    }
    #endregion
}
