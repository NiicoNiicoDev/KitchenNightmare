using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_MeatTenderizer : SCR_CustomColliderAttack
{
    [Header("Meat Tenderizer Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject pointLight;

    public override void Attack()
    {
        base.Attack();
        SCR_CameraShake camera = Camera.main.GetComponent<SCR_CameraShake>();
        camera.CallCameraShake(1.0f, 1.5f);

        for (int i = 0; i < trigger.hitObjects.Count; i++)
        {
            SCR_EnemyStats enemy = trigger.hitObjects[i].GetComponent<SCR_EnemyStats>();
            enemy.TakeDamage(damage);
        }

        for (int i = 0; i < trigger.doors.Count; i++)
        {
            trigger.doors[i].SmashOpenDoor(playerTransform.forward);
        }
    }

    public override void PlayVFX()
    {
        //base.PlayVFX();

        Vector3 offset = Vector3.up * 10.0f;
        RaycastHit hit;
        if (Physics.Raycast(VFX_Spawn_Point.position + offset, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            VFX_Spawn_Point.position = hit.point;
            base.PlayVFX();
        }
    }
}
