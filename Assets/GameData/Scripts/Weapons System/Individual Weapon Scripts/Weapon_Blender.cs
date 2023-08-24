using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Blender : SCR_SphericalAttack
{
    [Header("Blender Settings")]
    [SerializeField] private int damageToPlayer;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void Attack()
    {
        base.Attack();

        for (int i = 0; i < hitObjects.Length; i++)
        {
            hitObjects[i].GetComponent<SCR_EnemyStats>().TakeDamage(damage);
        }
    }

    public override void SetWeaponInactive()
    {
        base.SetWeaponInactive();
    }

    public override void SetWeaponActive()
    {
        base.SetWeaponActive();
    }
}
