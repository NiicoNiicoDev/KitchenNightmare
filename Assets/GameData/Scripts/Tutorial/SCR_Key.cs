using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Key : MonoBehaviour
{
    [SerializeField] private GameObject rottenWasabiPrefab;
    [SerializeField] private GameObject rottenWasabiDisplayTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SCR_TutorialDoorLock>().HasCollectedKey();
            Instantiate(rottenWasabiPrefab, Vector3.zero, Quaternion.identity);
            rottenWasabiDisplayTrigger.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.rotation *= Quaternion.Euler(0, 90 * Time.fixedDeltaTime, 0);
    }
}
