using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

//controls the game behaviour after defeating the boss
public class SCR_OpenStore : MonoBehaviour
{
    //reference to the defeated boss, in its "good" form
    [SerializeField] private GameObject defeatedBoss;

    //position of the shopkeeper when the round ends
    [SerializeField] private Transform shopkeeperPosition;

    //how quickly the player moves into the shop position and the camera changes to first person
    [SerializeField] private float cameraSwitchSpeed = 1f;

    //the shopkeeper's dialogue prior to opening the store menu
    [TextArea]
    [SerializeField] private string[] shopkeeperDialogue;

    //the name of the shopkeeper for the round
    [SerializeField] private string shopkeeperName;

    [SerializeField] private SCR_StoreUIHandler uIHandler;

    //the player's location when the shop menu is loaded
    private Transform playerPos;

    //position of the first person camera
    private Transform FPCameraPos;

    //detects if the store is open to prevent it being opened multiple times
    private bool bStoreOpened = false;

    //reference to the displayed UI when the shop is opened
    private GameObject shopUI;

    //reference to the HUD
    [SerializeField] private GameObject HUD;

    //reference to the text box used to display the shopkeeper's dialogue
    private GameObject textBox;

    //reference to the text component of the shopkeeper's dialogue
    private TextMeshProUGUI dialogue;

    //reference to the player
    private GameObject player;

    private NavMeshAgent navMeshAgentPlayer;

    private GameObject[] enemies;

    private bool bSequenceFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        //initialise the game objects and scripts
        player = GameObject.FindWithTag("Player");

        navMeshAgentPlayer = player.GetComponent<NavMeshAgent>();

        FPCameraPos = GameObject.Find("FPCameraPosition").transform;

        playerPos = GameObject.Find("PlayerEndPosition").transform;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        /*foreach (GameObject go in enemies)
        {
            Debug.Log(go.name);
        }*/

        //HUD = GameObject.Find("HUD");

        textBox = GameObject.Find("ShopkeeperDialogue");
        shopUI = GameObject.Find("ShopUI");
        shopUI.SetActive(false); //turn off the shop ui on start

        dialogue = textBox.GetComponentInChildren<TextMeshProUGUI>();
        textBox.SetActive(false); //turn of the text box on start
    }

    //called once the boss is defeated
    public void OpenStore()
    {
        //if the store isn't already open
        if(!bStoreOpened)
        {
            //set the bool to true to prevent the store from being opened multiple times
            bStoreOpened = true;

            //set the time scale to 0 to pause the game (running update functions are handled in respective scripts)
            /*Time.timeScale = 0f;*/

            foreach(GameObject enemy in enemies)
            {
                if (!enemy.IsDestroyed())
                {
                    enemy.GetComponent<SCR_EnemyStats>().TakeDamage(enemy.GetComponent<SCR_EnemyStats>().CurrentHealth); //Destroys all enemies on screen
                }
            }
            
            //begin the opening store process
            StartCoroutine("StoreOpeningProcess");
        }
    }
    
    //the full process of opening the shop
    IEnumerator StoreOpeningProcess()
    {
        AnimatorClipInfo[] clipInfo = player.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);

        float clipLength = clipInfo[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        //spawn the defeated boss (i.e., the good version) at the set spot
        Instantiate(defeatedBoss, shopkeeperPosition.position, shopkeeperPosition.rotation);

        //disable the character controller on the player so the player can be moved
        //player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<PlayerInput>().enabled = false;

        player.GetComponent<Animator>().SetBool("isMoving", true);

        //continue moving the player until they are virtually at the correct position (with a 0.1 error margin)
        while (Vector3.Distance(player.transform.position, playerPos.position) > 1f)
        {
            //move the player. Note that game is paused, so Time.unscaledDeltaTime is used instead of Time.deltaTime
            navMeshAgentPlayer.SetDestination(playerPos.position);

            //wait a frame to prevent the looping code from running too fast (which can crash unity)
            yield return null;
        }

        //re-enable the character controller
        //player.GetComponent<CharacterController>().enabled= true;
        player.GetComponent<Animator>().SetBool("isMoving", false);

        //Time.timeScale = 0f;

        //turn the player so they are looking at the shopkeeper
        //player.transform.LookAt(shopkeeperPosition);

        //rotate the camera to the player's rotation
        //TODO: make this transition smoother using Quaternion.Lerp similar to the loop below
        /*Camera.main.gameObject.transform.rotation = player.transform.rotation;

        Camera.main.orthographic = false;

        //keep moving the camera until they reach the correct position
        while(Camera.main.gameObject.transform.position != FPCameraPos.position)
        {
            //move the main camera
            Camera.main.gameObject.transform.position = Vector3.Lerp(Camera.main.gameObject.transform.position, FPCameraPos.position, cameraSwitchSpeed * Time.unscaledDeltaTime);

            //if the camera is within 0.1 units of the correct position, move the camera to the correct position
            //this prevents the loop from taking a long time to break
            if(Vector3.Distance(Camera.main.gameObject.transform.position, FPCameraPos.position) < 0.1f)
            {
                Camera.main.gameObject.transform.position = FPCameraPos.position;
            }

            //wait a frame to prevent looping too quickly
            yield return null;
        }*/

        //hide the HUD
        HUD.SetActive(false);

        //display the text box object
        textBox.SetActive(true);

        bSequenceFinished = true;

        //display the text items 1 by 1
        foreach (string text in shopkeeperDialogue)
        {
            //display the text
            dialogue.text = shopkeeperName + ": "+ text;

            //wait until the player presses space before displaying the next line
            while(!Input.GetKeyDown(KeyCode.Space) )
            {
                yield return null;
            }

            yield return null;
        }

        //hide the text box
        textBox.SetActive(false);

        //display the shop UI, with the weapons & weapon upgrades available to purchase
        shopUI.SetActive(true);

        uIHandler.CheckButtons();

        uIHandler.OpenWeaponSelection();
    }

    private void Update()
    {
        if (bSequenceFinished)
        {
            player.transform.LookAt(shopkeeperPosition.position);
        }
    }
}
