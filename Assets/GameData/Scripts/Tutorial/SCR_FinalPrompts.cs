using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FinalPrompts : MonoBehaviour
{
    [SerializeField] private bool isExitTrigger = false;

    [SerializeField] private GameObject canvas;
    [SerializeField] private bool isInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isExitTrigger)
            {
                canvas.SetActive(true);
                isInTrigger = true;
            }
            else
            {
                FindObjectOfType<SCR_TutorialManager>().DisplayText(4);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isExitTrigger)
            {
                canvas.SetActive(false);
                isInTrigger = false;
            }
        }
    }

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<SCR_TutorialManager>().EndTutorial();
        }

        if (canvas != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
        }
    }
}
