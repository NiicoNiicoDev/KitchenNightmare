using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_InstantKettleSteam : MonoBehaviour
{
    [SerializeField] private ParticleSystem steam;
    [SerializeField] private GameObject healthObj;
    [SerializeField] private SCR_TutorialManager tutorialManager;

    [SerializeField] private SCR_OpenDoor door1;
    [SerializeField] private SCR_OpenDoor door2;

    private bool activated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            if (!activated && !tutorialManager.thoughtBubble.isRunning)
            {
                steam.Play();
                SCR_PlayerStats player = other.GetComponent<SCR_PlayerStats>();
                player.TakeDamage(25);
                player.StunPlayer(6.0f, false);
                player.godModeActive = true;

                activated = true;

                door1.isLocked = false;
                door2.isLocked = false;

                FindObjectOfType<SCR_TutorialManager>().DisplayText(2);
                healthObj.SetActive(true);
            }
        }
    }

    private void LateUpdate()
    {
        if (healthObj == null)
        {
            tutorialManager.DisplayText(3);
            Destroy(gameObject);
        }
    }
}
