using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_BasicAttack : SCR_BaseWeapon
{
    [Header("Basic Attack Variables")]
    [SerializeField] protected float attackRange;
    public override void Attack()
    {
        base.Attack();

        //RaycastHit[] enemiesInRange = Physics.RaycastAll(playerTransform.position, playerTransform.forward, attackRange, enemyLayer, QueryTriggerInteraction.Ignore);
        RaycastHit[] enemiesInRange = Physics.SphereCastAll(playerTransform.position, 0.5f, playerTransform.forward, attackRange, enemyLayer, QueryTriggerInteraction.Ignore);


        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            SCR_EnemyStats enemy = enemiesInRange[i].transform.GetComponent<SCR_EnemyStats>();
            enemy.TakeDamage(damage);
        }
    }
}
