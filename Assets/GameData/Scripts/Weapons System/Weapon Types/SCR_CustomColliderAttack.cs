using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CustomColliderAttack : SCR_BaseWeapon
{
    [Header("Custom Collider Settings")]
    [Tooltip("Must have the collider attached to child")]
    [SerializeField] private GameObject meshPrefab;

    protected Mesh_Trigger trigger;
    [SerializeField] protected GameObject meshObject;

    protected override void Awake()
    {
        base.Awake();

        if (meshPrefab == null)
        {
            throw new System.Exception("Mesh Prefab is Required!");
        }

        meshObject = Instantiate(meshPrefab, playerTransform.position, Quaternion.identity, playerTransform);
        meshObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        trigger = meshObject.GetComponentInChildren<Mesh_Trigger>();
        meshObject.SetActive(false);
    }
    public override void Attack()
    {
        base.Attack();
    }

    public override void SetWeaponInactive()
    {
        base.SetWeaponInactive();

        meshObject.SetActive(false);
    }

    public override void EnableCustomMesh()
    {
        base.EnableCustomMesh();

        if (meshObject != null)
        {
            meshObject.SetActive(true);
            trigger.hitObjects.Clear();
        }
    }
}
