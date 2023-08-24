using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CooldownPickup : MonoBehaviour
{

    [SerializeField] private GameObject meshObj;
    [SerializeField] private ParticleSystem particles;
    BoxCollider collider;

    [HideInInspector] public SCR_CooldownSpawner spawner;
    [SerializeField] private float respawnDelay = 120.0f;
    private bool onCooldown = false;
    [SerializeField] private float rotateSpeed = 0.8f;
    [Tooltip("Time in seconds")]
    [SerializeField] private int cooldownReduction = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<SCR_WeaponHandler>().ReduceWeaponCooldowns(cooldownReduction);
            SetActive(false);
            StartCoroutine(WaitUntilRespawn());
        }
    }

    public void SetActive(bool value)
    {
        if (collider == null)
        {
            collider = GetComponent<BoxCollider>();
        }

        meshObj.SetActive(value);
        collider.enabled = value;
        onCooldown = !value;

        if (value)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (!onCooldown)
        {
            transform.rotation *= Quaternion.Euler(0, 90 * rotateSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private IEnumerator WaitUntilRespawn()
    {
        if (spawner != null) { spawner.SetNewPosition(gameObject); }

        yield return new WaitForSecondsRealtime(respawnDelay);
        SetActive(true);
    }
}
