using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_SafeRoom : MonoBehaviour
{
    [SerializeField] private SCR_OpenDoor[] doors;
    private List<SCR_OpenDoor> doorsToUnlock = new List<SCR_OpenDoor>();

    private Transform player;
    private PSM_MovementStateMachine psm;

    [SerializeField] private GameObject promptPrefab;
    private List<GameObject> promptPoints = new List<GameObject>();

    [SerializeField] private GameObject displayCanvas;
    [SerializeField] private Animation anim;

    [SerializeField] private Transform spawn;

    bool displayShowing = false;

    [HideInInspector] public bool collectedFoodOrder = false;

    [SerializeField] private bool isTutorialLevel = false;

    private void Start()
    {
        if (isTutorialLevel) { return; }

        List<GameObject> prompts = new List<GameObject>();

        //Find the doors that pathfinding unlocked earlier and caches them
        for (int i = 0; i < doors.Length; i++)
        {
            if (!doors[i].isLocked)
            {
                doorsToUnlock.Add(doors[i]);
                if (!prompts.Contains(doors[i].transform.parent.gameObject))
                {
                    GameObject obj = Instantiate(promptPrefab, doors[i].transform.parent.position + doors[i].transform.parent.forward * -1.0f, Quaternion.identity, doors[i].transform.parent);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    prompts.Add(doors[i].transform.parent.gameObject);
                    promptPoints.Add(obj);
                }
            }
            doors[i].isLocked = true;
        }
        RecallPlayer();
    }

    public void UnlockDoors()
    {
        CloseDisplay();

        foreach (SCR_OpenDoor door in doorsToUnlock)
        {
            door.isLocked = false;
        }

        //Destroy(displayCanvas);
        //Destroy(this);

        displayCanvas.SetActive(false);
        foreach(GameObject prompt in promptPoints)
        {
            prompt.SetActive(false);
        }

        FindObjectOfType<RoundTimer>().OnRoundStart();
        GameManager.gameManager.bRoundStarted = true;
    }

    private void Update()
    {
        if (displayShowing) 
        {
            displayCanvas.transform.LookAt(Camera.main.transform);
            return; 
        }
    }

    public void QueryGameStart()
    {
        if (!collectedFoodOrder)
        {
            RemindPlayerToGetOrder();
            return;
        }

        anim.Play();
        
        displayShowing = true;

        displayCanvas.SetActive(true);
        Cursor.visible = true;

        psm.canMove = false;
    }

    public void CloseDisplay()
    {
        displayCanvas.SetActive(false);
        Cursor.visible = false;

        psm.canMove = true;
        displayShowing = false;
    }

    public void ResetSafeRoom()
    {
        foreach (GameObject prompt in promptPoints)
        {
            prompt.SetActive(true);
        }
    }

    private void RemindPlayerToGetOrder()
    {
        string message = "I should check out the food order first...";

        SCR_ThoughtBubble bubble = FindObjectOfType<PSM_InputHandler>().thoughtBubble;

        StartCoroutine(bubble.DisplayText(message, 2.0f));
    }

    public void RecallPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            psm = player.GetComponent<PSM_MovementStateMachine>();
        }
        psm.agent.enabled = false;

        player.transform.position = spawn.position;
        psm.canMove = true;

        psm.agent.enabled = true;
    }

    public void PlayInteractSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }
}
