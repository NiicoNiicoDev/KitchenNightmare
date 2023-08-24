using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_RottenEnemyHint : MonoBehaviour
{
    private float waitTime = 2.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SCR_TutorialManager>().DisplayText(6, waitTime);
            Destroy(gameObject);
        }
    }
}
