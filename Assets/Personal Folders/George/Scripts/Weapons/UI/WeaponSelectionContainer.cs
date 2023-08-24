using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionContainer : MonoBehaviour
{
    WeaponShopHandler shopHandler;

    SCR_BaseWeapon targetWeapon;
    [SerializeField] int indexInSelectionMenu;
    [SerializeField] Image image;

    [SerializeField] Button button;

    [SerializeField] private GameObject basicWeapons;
    [SerializeField] private GameObject abilityWeapons;


    private void Awake()
    {
        shopHandler = FindObjectOfType<WeaponShopHandler>();
        
        targetWeapon = shopHandler.availableWeapons[indexInSelectionMenu].GetComponent<SCR_BaseWeapon>();

        image.sprite = targetWeapon.weaponSprite;

        button = GetComponentInChildren<Button>();

        if (targetWeapon.IsUnlocked == false)
        {
            Color colour = image.color;
            colour.a = 0.4f;
            image.color = colour;
            button.gameObject.SetActive(false);
            GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
        }
    }

    private void LateUpdate()
    {
        if (button.gameObject.activeInHierarchy != targetWeapon.IsUnlocked)
        {
            button.gameObject.SetActive(targetWeapon.IsUnlocked);
            image.color = new Color(1,1,1,1);
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void PlayInteractSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }
}
