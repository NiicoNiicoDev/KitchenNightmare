using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class SCR_CriticalDamageAnim : MonoBehaviour
{
    private Animator criticalDamageAnimator;
    [SerializeField] private Slider healthBar;
    void Start()
    {
        criticalDamageAnimator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar.value < 30)
        {
            criticalDamageAnimator.SetBool("CriticalDamage", true);
        }
        else
        {
            criticalDamageAnimator.SetBool("CriticalDamage", false);
        }
    }
}
