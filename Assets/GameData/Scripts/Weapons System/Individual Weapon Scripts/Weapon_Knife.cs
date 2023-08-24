using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Knife : SCR_LineAttack
{
    List<GameObject> hitObjects;

    [SerializeField] private GameObject knifeProjectile;
    public override void Attack()
    {
        base.Attack();
        
        knifeProjectile.GetComponent<Weapon_KnifeProjectile>().knifeParent = this.gameObject;
        GameObject projectile = Instantiate(knifeProjectile, playerTransform.position + new Vector3(0, 0.5f, 0) + playerTransform.forward * 0.75f, playerTransform.rotation * Quaternion.Euler(90, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") == true)
        {
            hitObjects.Add(collision.gameObject);
            collision.transform.SetParent(transform);
            collision.transform.GetComponent<SCR_EnemyStats>().TakeDamage(damage);
        }
    }

    public override void OnAttackEnded()
    {
        foreach (GameObject hitObject in hitObjects)
        {
            hitObject.transform.SetParent(null);
        }

        hitObjects.Clear();
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
