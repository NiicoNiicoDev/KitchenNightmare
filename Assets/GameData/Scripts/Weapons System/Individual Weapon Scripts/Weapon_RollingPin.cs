using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon_RollingPin : SCR_LineAttack
{
    [SerializeField] private GameObject rollingPinProjectile;

    Vector3 endPos;

    public override void Attack()
    {
        base.Attack();
        rollingPinProjectile.GetComponent<Weapon_RollingPinProjectile>().rollingPinParent = this.gameObject;
        Instantiate(rollingPinProjectile, playerTransform.position, playerTransform.rotation * Quaternion.Euler(0,0,90));
    }

    public int Damage()
    {
        return damage;
    }

    public float AttackRange()
    {
        return attackRange;
    }
}


