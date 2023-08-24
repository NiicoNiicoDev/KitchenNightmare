using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShopAnims : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Animator upgradesAnimator;
    [SerializeField] private Animator weaponAnimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartShrinkingUpgradesAnim()
    {
        upgradesAnimator.SetTrigger("ButtonPressed");
    }

    public void StartGrowUpgradesAnim()
    {
        upgradesAnimator.SetTrigger("ButtonUnpressed");
    }

    public void StartShrinkingWeaponsAnim()
    {
        weaponAnimator.SetTrigger("ButtonPressed");
    }

    public void StartGrowWeaponsAnim()
    {
        weaponAnimator.SetTrigger("ButtonUnpressed");
    }
}
