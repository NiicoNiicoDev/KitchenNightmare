using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the behaviour of health power-ups
public class SCR_HealthPowerUp : MonoBehaviour
{
    //how much the player's health increases after picking up the power-up
    [SerializeField] private int healthIncrease = 1;

    //detects if the health script increased the player health
    private bool bIncreaseHealth = false;

    private SphereCollider collider;
    private GameObject mesh;
    [SerializeField] private ParticleSystem particleSystem;

    [SerializeField] private float respawnTimer = 1.0f;
    private WaitForSecondsRealtime delay;

    bool onCooldown = false;

    [SerializeField] private bool shouldRespawn = true;


    private void OnTriggerEnter(Collider other)
    {
        //if the player has collided with the power-up
        if (other.CompareTag("Player"))
        {
            if (collider == null)
            {
                collider = GetComponent<SphereCollider>();
                mesh = transform.GetChild(0).gameObject;
                delay = new WaitForSecondsRealtime(respawnTimer);
            }

            SCR_PlayerStats healthScript = other.GetComponent<SCR_PlayerStats>();

            SCR_PoisonMechanics poisonScript = other.GetComponent<SCR_PoisonMechanics>();

            bIncreaseHealth = healthScript.baseHealth > healthScript.currentHealth;

            //if the player's health WAS increased
            if (bIncreaseHealth)
            {
                //attempt to increase the player's health
                healthScript.AddHealth(healthIncrease);

                poisonScript.RemovePoison();

                if (!shouldRespawn) { Destroy(gameObject); }

                StartCoroutine(SetInactive());
            }
        }
    }

    private void FixedUpdate()
    {
        if (!onCooldown)
        {
            transform.rotation *= Quaternion.Euler(0, 90 * Time.fixedDeltaTime, 0);
        }
    }

    private IEnumerator SetInactive()
    {
        collider.enabled = false;
        mesh.SetActive(false);
        onCooldown = true;
        particleSystem.Stop();

        yield return delay;

        collider.enabled = true;
        mesh.SetActive(true);
        onCooldown = false;
        particleSystem.Play();
    }
}
