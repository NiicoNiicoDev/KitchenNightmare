using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_EnemyTutorialTrigger : MonoBehaviour
{
    private float waitTime = 2.5f;
    [SerializeField] private SCR_OpenDoor[] doors;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SCR_TutorialManager>().DisplayText(5, waitTime);
            StartCoroutine(WaitTime());
        }
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        foreach (SCR_OpenDoor door in doors)
        {
            door.UnlockDoor();
        }
        Destroy(gameObject);
    }
}
