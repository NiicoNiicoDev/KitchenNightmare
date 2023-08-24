using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponShopHandler : MonoBehaviour
{
    public GameObject[] availableWeapons;

    [SerializeField] private int confidenceCurrency = 0;
    [SerializeField] private TextMeshProUGUI confidenceCurrencyDisplay;


    private void Awake()
    {
        availableWeapons = FindObjectOfType<SCR_WeaponHandler>().Weapons;
    }

    // Start is called before the first frame update
    void Start()
    {
        confidenceCurrency = GameManager.gameManager.currentConfidence;
        confidenceCurrencyDisplay.text = "Confidence Currency: " + confidenceCurrency;
    }

    // Update is called once per frame
    /*void Update()
    {
        confidenceCurrency = playerStats.ConfidenceCurreny;
        confidenceCurrencyDisplay.text = "Confidence Currency: " + confidenceCurrency;
    }*/

    public void UnlockWeapon(int weaponIndex)
    {
        SCR_BaseWeapon targetWeapon = availableWeapons[weaponIndex].GetComponent<SCR_BaseWeapon>();
       targetWeapon.UnlockWeapon();

        transform.GetChild(0).GetChild(weaponIndex + 1).GetComponent<WeaponShopContainer>().UpdateUI();

        if (weaponIndex == 6)
        {
            transform.GetChild(0).GetChild(weaponIndex + 1).GetComponent<WeaponShopContainer>().upgradeButton.gameObject.SetActive(false);
        }

        transform.GetChild(0).GetChild(weaponIndex + 1).GetComponent<WeaponShopContainer>().OnUpgradeUpdateUI(targetWeapon.CurrentUpgradeLevel, targetWeapon.UpgradeCosts[targetWeapon.CurrentUpgradeLevel]);
    }

    public void UpgradeWeapon(int weaponIndex)
    {
  
        SCR_BaseWeapon targetWeapon = availableWeapons[weaponIndex].GetComponent<SCR_BaseWeapon>();
        WeaponShopContainer container = transform.GetChild(0).transform.GetChild(weaponIndex+1).GetComponent<WeaponShopContainer>();

        if (targetWeapon == null)
        {
            return;
        }

        Debug.Log(targetWeapon);
        
        if (confidenceCurrency <= targetWeapon.UpgradeCosts[targetWeapon.CurrentUpgradeLevel])
        {
            return;
        }
        
        targetWeapon.UpgradeWeapon();

        if (targetWeapon.CurrentUpgradeLevel >= targetWeapon.UpgradeCosts.Length) 
        {
            container.OnUpgradeUpdateUI(targetWeapon.CurrentUpgradeLevel, 0);
            return; 
        } 
        else
        {
            container.OnUpgradeUpdateUI(targetWeapon.CurrentUpgradeLevel, targetWeapon.UpgradeCosts[targetWeapon.CurrentUpgradeLevel]);
        }
    }
}
