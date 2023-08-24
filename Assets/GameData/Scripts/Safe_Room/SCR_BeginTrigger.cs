using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_BeginTrigger : MonoBehaviour
{
    SCR_SafeRoom safeRoom;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (safeRoom == null)
            {
                safeRoom = FindObjectOfType<SCR_SafeRoom>();
            }

            safeRoom.QueryGameStart();
        }
    }
}
