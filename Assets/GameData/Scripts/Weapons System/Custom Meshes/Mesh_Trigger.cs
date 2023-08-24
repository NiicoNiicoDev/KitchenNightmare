using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Trigger : MonoBehaviour
{
    public List<Collider> hitObjects = new List<Collider>();
    public List<SCR_OpenDoor> doors = new List<SCR_OpenDoor>();

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask doorLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (!InCorrectLayer(other.gameObject)) { return; }

        hitObjects.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!InCorrectLayer(other.gameObject)) { return; }

        hitObjects.Remove(other);
    }

    private bool InCorrectLayer(GameObject obj)
    {
        int layerMask = (1 << obj.layer);

        if (layerMask == doorLayer)
        {
            SCR_OpenDoor door = obj.GetComponent<SCR_OpenDoor>();
            if (!door.isLocked)
            {
                doors.Add(door);
            }
        }
        return layerMask == enemyLayer;
    }
}
