using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SCR_TutorialManager : MonoBehaviour
{
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Walking Through Doorway")]
    [SerializeField] private Animation startAnimation;
    [SerializeField] private Transform desiredPoint;
    [SerializeField] private float animMoveSpeed;

    //Player Variables
    [Header("Player Variables")]
    [SerializeField] private Transform playerTransform;
    private Animator animController;
    private PlayerInput playerInput;
    private NavMeshAgent navAgent;
    private PSM_MovementStateMachine playerMovement;

    [Header("Thought Bubble")]
    [SerializeField] private string[] tutorialPrompts;
    public SCR_ThoughtBubble thoughtBubble;

    [SerializeField] private Image fadeToBlack;
    [SerializeField] private GameObject HUD;
    private void Start()
    {
        animController = playerTransform.GetComponent<Animator>();
        playerInput = playerTransform.GetComponent<PlayerInput>();
        navAgent = playerTransform.GetComponent<NavMeshAgent>();
        playerMovement = playerTransform.GetComponent<PSM_MovementStateMachine>();
    }

    public void EnablePlayerControls()
    {
        animController.enabled = true;
        playerInput.enabled = true;
        playerMovement.canMove = true;
    }

    public void EndCameraAnimation(GameObject camera)
    {
        camera.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        StartCoroutine(StartAnimation());
    }
    private IEnumerator StartAnimation()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        startAnimation.Play();
        while (Vector3.Distance(playerTransform.position, desiredPoint.position) > 0.25f)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, desiredPoint.position, animMoveSpeed * Time.fixedDeltaTime);
            yield return null;
        }
        startAnimation.enabled = false;

        StartCoroutine(thoughtBubble.DisplayText(tutorialPrompts[0], 1.0f));

        EnablePlayerControls();
    }

    public void DisplayText(int textIndex, float displayTime = 2.5f)
    {
        if (thoughtBubble.isRunning) 
        { 
            thoughtBubble.QueueThoughtBubble(tutorialPrompts[textIndex], displayTime); 
        }
        else
        {
            StartCoroutine(thoughtBubble.DisplayText(tutorialPrompts[textIndex], displayTime));
        }
    }

    public void EndTutorial()
    {
        HUD.SetActive(false);
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        fadeToBlack.gameObject.SetActive(true);
        while (fadeToBlack.color.a < 1.0f)
        {
            float alpha = fadeToBlack.color.a + (0.3f * Time.fixedDeltaTime);
            Color colour = fadeToBlack.color;
            colour.a = alpha;

            fadeToBlack.color = colour;

            yield return null;
        }

        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// A Coroutine that waits for given time then calls a given function
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="function"></param>
    /// <returns></returns>
    private IEnumerator WaitForSeconds(float seconds, Func function)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(seconds);
        yield return delay;
        function();
    }
    private delegate void Func();
}
