using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LineAttack : SCR_BaseWeapon
{
    [Header("Line Attack Variables")]
    [SerializeField] protected Vector3 attackHitbox;
    [Space(10)]
    [SerializeField] protected float attackRange;

    public override void Attack()
    {
        base.Attack();
    }

}
