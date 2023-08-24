using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_MortarPestle : SCR_SphericalAttack
{
    [Header("Mortar and Pestle Settings")]
    [Tooltip("Time in seconds to draw in enemies")]
    [SerializeField] private float attractTime = 1.0f;
    [SerializeField] private float stunTime = 1.0f;
    [SerializeField] private float knockbackForce = 1.0f;
    [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private GameObject pushParticleEffectPrefab;

    private float attractForce = 1.0f;

    private Vector3[] newEnemyPositions;

    protected override void Awake()
    {
        base.Awake();

        attractForce = attractForce / attractTime;
    }
    public override void Attack()
    {
        base.Attack();
        StartCoroutine(DrawInEnemies());
    }

    private IEnumerator KnockbackEnemies()
    {
        VFX_Prefab = pushParticleEffectPrefab;
        PlayVFX();

        newEnemyPositions = new Vector3[hitObjects.Length];

        for (int i = 0; i < hitObjects.Length; i++)
        {
            float distanceToPlayer = Vector3.Distance(hitObjects[i].transform.position, playerTransform.position);
            float relativeForce = knockbackForce / distanceToPlayer;
            Vector3 direction = hitObjects[i].transform.position - playerTransform.position;
            direction.Normalize();
            newEnemyPositions[i] = playerTransform.position + (direction * relativeForce);
        }

        int safety = 0;
        float timer = 0;

        while (timer < damageDelay)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < hitObjects.Length; i++)
            {
                hitObjects[i].transform.position = Vector3.Lerp(hitObjects[i].transform.position, newEnemyPositions[i], knockbackForce * Time.deltaTime);

                Debug.DrawLine(hitObjects[i].transform.position, newEnemyPositions[i], Color.red);
            }

            safety++;
            if (safety >= 1000)
            {
                break;
            }

            yield return null;
        }
        //yield return new WaitForSeconds(damageDelay);

        for (int j = 0; j < hitObjects.Length; j++)
        {
            hitObjects[j].GetComponent<SCR_EnemyStats>().TakeDamage(damage);
        }

        //Can be removed once animation is in use
        SetWeaponInactive();
    }

    private IEnumerator DrawInEnemies()
    {
        float pullDuration = attractTime;
        Vector3 previousPlayerPosition = playerTransform.position;

        while (pullDuration > 0.0f)
        {
            pullDuration -= Time.deltaTime;

            hitObjects = GetEnemiesHit();

            //Vector3 offset = playerTransform.position - previousPlayerPosition;
            for (int j = 0; j < hitObjects.Length; j++)
            {
                SCR_EnemyStats enemy = hitObjects[j].GetComponent<SCR_EnemyStats>();
                if (!enemy.IsStunned)
                {
                    enemy.StunEnemy(stunTime);
                }

                //hitObjects[j].transform.position += offset;
                if (Vector3.Distance(hitObjects[j].transform.position, playerTransform.position) > 1.0f)
                {
                    hitObjects[j].transform.position = Vector3.Lerp(hitObjects[j].transform.position, playerTransform.position, attractForce * Time.deltaTime);
                }
            }
            //previousPlayerPosition = playerTransform.position;
            playerTransform.position = previousPlayerPosition;
            yield return null;
        }

        StartCoroutine(KnockbackEnemies());
    }
}
