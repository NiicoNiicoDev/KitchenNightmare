using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TutorialDoorLock : MonoBehaviour
{ 
    private Transform playerTransform;
    private Vector3 doorPos;
    private SCR_TutorialManager tutorialManager;

    private float lastFlaggedDistance;
    private bool steppedAway = true;

    private bool hasKey = false;
    [SerializeField] private GameObject unlockDoorText;
    [SerializeField] private SCR_OpenDoor[] doors;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        doorPos = transform.position;
        tutorialManager = FindObjectOfType<SCR_TutorialManager>();
    }

    private void Update()
    {
        if (tutorialManager.thoughtBubble.isRunning) { return; }

        float distance = Vector3.Distance(playerTransform.position, doorPos);
        if (distance <= 1.0f && steppedAway)
        {

            if (hasKey)
            {
                if (!unlockDoorText.activeSelf) { unlockDoorText.SetActive(true); }
            }
            else
            {
                tutorialManager.DisplayText(1);
            }

            steppedAway = false;
            lastFlaggedDistance = distance;
        }

        if (!steppedAway && distance > lastFlaggedDistance)
        {
            steppedAway = true;
            if (unlockDoorText.activeSelf) { unlockDoorText.SetActive(false); }
        }

        if (hasKey && Input.GetKeyDown(KeyCode.E))
        {
            foreach (SCR_OpenDoor door in doors)
            {
                door.UnlockDoor();
            }
            Destroy(unlockDoorText);
            Destroy(this);
        }
    }

    public void HasCollectedKey()
    {
        hasKey = true;
    }

    private void FixedUpdate()
    {
        if (unlockDoorText.activeSelf)
        {
            unlockDoorText.transform.LookAt(Camera.main.transform);
        }
    }
}
