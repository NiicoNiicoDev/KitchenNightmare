using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FoodOrder : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.rotation *= Quaternion.Euler(0, 90 * Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SCR_SafeRoom>().collectedFoodOrder = true;
            FindObjectOfType<PSM_InputHandler>().pauseMenu.collectedOrder = true;
            Destroy(gameObject);
        }
    }
}
