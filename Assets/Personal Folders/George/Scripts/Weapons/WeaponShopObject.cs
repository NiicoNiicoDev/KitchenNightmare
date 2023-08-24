using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponShopObject : MonoBehaviour
{
    [SerializeField] TMP_Text interactionText;
    bool bCanInteract = false;

    [SerializeField] bool bIsWeaponShop;

    Transform cameraTransform;
    
    private void OnTriggerEnter(Collider other)
    {
        interactionText.gameObject.SetActive(true);
        if (other.gameObject.tag == "Player" && !GameManager.gameManager.bRoundStarted)
        {
            bCanInteract = true;
        }

        if (GameManager.gameManager.bRoundStarted)
        {
            GetComponentInChildren<TMP_Text>().text = "Round Has Started";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactionText.gameObject.SetActive(false);
        if (other.gameObject.tag == "Player")
        {
            bCanInteract = false;
            Cursor.visible = false;
        }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (!bCanInteract)
        {
            if (bIsWeaponShop)
                FindObjectOfType<WeaponShopHandler>().transform.GetChild(0).gameObject.SetActive(false);

            if (!bIsWeaponShop)
                FindObjectOfType<WeaponSelectionUI>().transform.GetChild(0).gameObject.SetActive(false);

            return;
        }
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        interactionText.gameObject.transform.LookAt(cameraTransform);
        interactionText.gameObject.transform.rotation *= Quaternion.Euler(0, 180, 0);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (bIsWeaponShop)
            {
                Cursor.visible = true;
                WeaponShopHandler handler = FindObjectOfType<WeaponShopHandler>();
                handler.transform.GetChild(0).gameObject.SetActive(true);

                for (int i = 0; i < handler.availableWeapons.Length; i++)
                {
                    WeaponShopContainer targetContainer = handler.transform.GetChild(0).GetChild(i).GetComponent<WeaponShopContainer>();
                    SCR_BaseWeapon targetWeapon = handler.availableWeapons[i].GetComponent<SCR_BaseWeapon>();

                    if (targetWeapon.CurrentUpgradeLevel < targetWeapon.UpgradeCosts.Length)
                        targetContainer.OnUpgradeUpdateUI(targetWeapon.CurrentUpgradeLevel, targetWeapon.UpgradeCosts[targetWeapon.CurrentUpgradeLevel]);
                    else
                    {
                        targetContainer.upgradeButton.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Cursor.visible = true;
                FindObjectOfType<WeaponSelectionUI>().transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;

            if (bIsWeaponShop)
            {
                FindObjectOfType<WeaponShopHandler>().transform.GetChild(0).gameObject.SetActive(false);
            } 
            else
            {
                FindObjectOfType<WeaponSelectionUI>().transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }
}
