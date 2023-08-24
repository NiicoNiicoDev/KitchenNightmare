using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deprecated_InputTestHangler : MonoBehaviour
{
    public void OnAttack()
    {
        FindObjectOfType<SCR_BaseWeapon>().PlayAttackAnimation();
    }
}
