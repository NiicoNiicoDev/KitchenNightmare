using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SphericalAttack : SCR_BaseWeapon
{
    [Header("Spherical Attack Variables")]
    [SerializeField] protected float radius;
    protected Collider[] hitObjects;


    public override void Attack()
    {
        base.Attack();

        hitObjects = GetEnemiesHit();
    }

    public override void DrawAttackArea()
    {
        base.DrawAttackArea();

        attackOutlineObject.transform.localScale *= radius;
    }

    protected Collider[] GetEnemiesHit()
    {
        return Physics.OverlapSphere(weaponTransform.position, radius, enemyLayer, QueryTriggerInteraction.Ignore);
    }
}
