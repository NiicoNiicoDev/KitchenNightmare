using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopContainer : MonoBehaviour
{
    WeaponShopHandler shopHandler;

    [SerializeField] int indexInShop;
    
    [Header("UI Components")]
    [SerializeField] Image weaponIcon;
    [SerializeField] TMP_Text weaponNameText;
    [SerializeField] TMP_Text currentLevelText;
    [SerializeField] TMP_Text upgradeCostText;
    [SerializeField] TMP_Text weaponDescriptionText;

    [SerializeField] string weaponDescription;

    [SerializeField] Button unlockButton;
    [SerializeField] public Button upgradeButton;

    private void Awake()
    {
        shopHandler = FindObjectOfType<WeaponShopHandler>();

        SCR_BaseWeapon targetWeapon = shopHandler.availableWeapons[indexInShop - 1].GetComponent<SCR_BaseWeapon>();
        //if (shopHandler.availableWeapons[indexInShop - 1] != null)
        SetWeaponIcon(targetWeapon.weaponSprite);

        SetWeaponNameText(shopHandler.availableWeapons[indexInShop - 1].name);

        SetWeaponLevelAndCostText(targetWeapon.CurrentUpgradeLevel, targetWeapon.UpgradeCosts[targetWeapon.CurrentUpgradeLevel]);

        weaponDescriptionText.text = weaponDescription;

        if (shopHandler.availableWeapons[indexInShop - 1].GetComponent<SCR_BaseWeapon>().IsUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            upgradeButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }

    public void SetWeaponNameText(string name)
    {
        weaponNameText.text = name;
    }

    void SetWeaponLevelAndCostText(int currentLevel, int upgradeCost)
    {
        if (shopHandler.availableWeapons[indexInShop - 1].GetComponent<SCR_BaseWeapon>().IsUnlocked == false)
        {
            currentLevelText.text = $"0";
            upgradeCostText.text = $"Unlock is free!";
            return;
        }
            
        if (currentLevel >= shopHandler.availableWeapons[indexInShop - 1].GetComponent<SCR_BaseWeapon>().UpgradeCosts.Length)
        {
            currentLevelText.text = $"Current Level: MAX";
            upgradeCostText.text = $"";
        }
        else
        {
            currentLevelText.text = $"Current Level: {currentLevel + 1}";
            upgradeCostText.text = $"Upgrade Cost: {upgradeCost}";
        }
    }

    public void OnUpgradeUpdateUI(int currentLevel, int upgradeCost)
    {
        SetWeaponLevelAndCostText(currentLevel, upgradeCost);
    }

    public void UpdateUI()
    {
        unlockButton.gameObject.SetActive(false);
        
        if (indexInShop != 6)
            upgradeButton.gameObject.SetActive(true);
    }

    public void PlayInteractSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }

}
