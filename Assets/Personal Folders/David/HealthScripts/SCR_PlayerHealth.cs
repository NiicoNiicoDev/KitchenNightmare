using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.Rendering;

//OBSOLETE SCRIPT, USE SCR_PLAYERSTATS
public class SCR_PlayerHealth : MonoBehaviour
{
    //starting health of the player (editable in inspector)
    //currently, is also the max health the player can reach (but can be changed if needed)
    [SerializeField] private int startingHealth = 1;

    //reference to the health bar used in the scene
    [SerializeField] private Slider healthBar;

    //reference to the fill area on the health bar
    [SerializeField] private Image healthBarFillArea;

    //reference to the game over screen (i.e., the panel, not just the text)
    [SerializeField] private GameObject gameOverScreen;

    //reference specifically to the game over text box
    [SerializeField] private GameObject gameOverTextBox;

    //selection of comments that are generated at random when the player dies on the game over screen
    //made the text box larger to make it easier to include longer comments
    [TextArea]
    [SerializeField] private string[] gameOverComments;

    //TODO: Set this to the active scoring script if there are multiple
    [SerializeField] private SCR_ScoringSystem scoringSystem;

    [SerializeField] private string deathTriggerName = "bDied";

    [SerializeField] private string respawnTriggerName = "bRespawned";

    //reference to the text in the game over text box
    private TextMeshProUGUI gameOverText;

    [HideInInspector] public bool bJustRespawned = false;

    //reference to ALL the scripts attached to the player (except this one)
    #region PLAYER DETAILS
    private NavMeshAgent movementScript;

    private SCR_PlayerAttack attackScript;

    private SCR_WeaponInventory inventoryScript;

    private SCR_StunPlayer stunMechanics;

    private CharacterController characterController;
    #endregion

    //reference to the image attached to the game over panel
    private Image gameOverBackground;

    //current health of the player
    private int currentHealth = 1;

    //location where the player will respawn after dying
    private Vector3 spawnPoint;

    //temp variable, reference to the original rotation of the player
    private Quaternion startRotation;

    [SerializeField] private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        //set spawn point to player's start position
        spawnPoint = transform.position;

        //initialise the player's start rotation (remove this when an animation has been made)
        startRotation = transform.rotation;

        //initialise the game over screen so it doesn't show when the scene loads
        gameOverText = gameOverTextBox.GetComponent<TextMeshProUGUI>();
        gameOverBackground = gameOverScreen.GetComponent<Image>();
        gameOverBackground.color = Color.clear;
        gameOverScreen.SetActive(false);

        //initialise the player scripts
        attackScript = GetComponent<SCR_PlayerAttack>();
        movementScript = GetComponent<NavMeshAgent>();
        inventoryScript = GetComponent<SCR_WeaponInventory>();
        stunMechanics = GetComponent<SCR_StunPlayer>();
        characterController = GetComponent<CharacterController>();

        //set the current health to the starting health
        currentHealth = startingHealth;

        if (healthBar)
        {
            //stops the player from changing the slider values by clicking on it
            healthBar.enabled = false;
        }

        //updates the health bar
        UpdateHealthBar();

        //playerAnimator = GetComponent<Animator>();
    }

    //called when the player is hit by an enemy (or hits a hazard)
    public void Damage(int healthLost)
    {
        //subtract health lost from current health
        currentHealth -= healthLost;

        //updates the health bar
        UpdateHealthBar();

        Debug.Log("Health remaining = " + currentHealth);

        //if the health has dropped to 0 (or less), then respawn the player
        //TODO: Trigger a respawn sequence instead of respawning the player instantly
        if (currentHealth <= 0)
        {
            StartCoroutine("Respawn");
        }
    }

    //called when the player gains health from a power-up
    //returns a bool to let the power-up script know whether the player's health was increased or not
    public bool IncreaseHealth(int healthIncrease)
    {
        //if the additional health increase sets the health below (or at) the starting health
        if(currentHealth + healthIncrease <= startingHealth)
        {
            //increase the player's health by the health increase
            currentHealth += healthIncrease;
            UpdateHealthBar();

            Debug.Log("New Health: " + currentHealth);

            //confirm to the script which called the function that the health was increased
            return true;
        } 
        else if(currentHealth < startingHealth) //if the health increase sets the health above the starting health but the current health is less than the starting health
        {
            //max out the player's health instead of adding the health increase to ensure it doesn't exceed the maximum health
            currentHealth = startingHealth;
            UpdateHealthBar();

            Debug.Log("New Health: " + currentHealth);

            //confirm to the script which called the function that the health was increased
            return true;
        }

        Debug.Log("Health already maxed out");

        //if the health was already full, confirm to the original script that the health was not increased
        return false;
    }

    //called when the player is "dead"
    IEnumerator Respawn()
    {
        //this allows the bAttacking bool to return back to false in player attack, which led to nearby boiling pots triggering on respawn
        yield return null;

        //disable all player scripts (except this one)
        TogglePlayerScripts(false);

        scoringSystem.PauseTimer();

        AnimatorClipInfo[] clipInfo = playerAnimator.GetNextAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);

        playerAnimator.SetTrigger(deathTriggerName);

        clipInfo = playerAnimator.GetCurrentAnimatorClipInfo(0);

        //once player has "fallen", wait 2 seconds to create a "dramatic" effect
        yield return new WaitForSeconds(clipInfo[0].clip.length);

        if (gameOverScreen)
        {
            //hide the game over text for now
            gameOverTextBox.SetActive(false);

            //make sure the game over background is clear to begin with, as a fade effect will play
            gameOverBackground.color = Color.clear;

            //display the game over screen
            gameOverScreen.SetActive(true);

            //get the background colour and store it in a temp variable. This is because the "a" value of the game over screen cannot be modified directly
            Color background = gameOverBackground.color;

            //create a fade effect
            for (float i = 0f; i <= 1f; i += 0.02f)
            {
                //gradually decrease the transparency of the game over screen over time until it is opaque
                background.a = i;
                gameOverBackground.color = background;

                //this prevents the risk of unity crashing by running the iterations too quickly
                yield return null;
            }

            //display the game over text and generate a line at random from the list of comments
            gameOverTextBox.SetActive(true);
            gameOverText.text = gameOverComments[Random.Range(0, gameOverComments.Length)];


            playerAnimator.SetTrigger(respawnTriggerName);

            //wait 5 seconds so the player has time to read the text
            yield return new WaitForSeconds(5f);
        }

        //hide the game over screen
        gameOverScreen.SetActive(false);

        //reset the player's rotation
        transform.rotation = startRotation;

        //return the player to the current spawn point
        transform.position = spawnPoint;

        //set the spawned bool to true
        //bJustRespawned = true;

        //reset the player's health
        currentHealth = startingHealth;

        //update the health bar to reflect the new health of the player
        UpdateHealthBar();

        //enable the player scripts again
        TogglePlayerScripts(true);
        //reset the weapon attack limits
        attackScript.ResetWeapons();

        scoringSystem.ContinueTimer();
    }

    //this is included due a bug in which the player sometimes did not return to the spawn point in Respawn()
    /*private void LateUpdate()
    {
        //if the player has just respawned
        if(bJustRespawned)
        {
            //move them back to the spawn point
            transform.position = spawnPoint;

            if (transform.position == spawnPoint)
            {
                //disable the spawn bool else the player will not be able to move
                bJustRespawned = false;
            }
        }
    }*/

    //toggles the player scripts on or off depending on the bool input
    void TogglePlayerScripts(bool newState)
    {
        //toggle each script to the new state
        attackScript.enabled = newState;
        movementScript.enabled = newState;
        stunMechanics.enabled = newState;
        inventoryScript.enabled = newState;
        //characterController.enabled = newState;
    }

    //called when the health has been changed
    void UpdateHealthBar()
    {
        if (healthBar)
        {
            //adjust the health bar value to current health / max health
            //also ensure that the health values are casted to floats else it will return 0 or 1
            healthBar.value = (float)currentHealth / (float)startingHealth;

            Debug.Log(healthBar.value);

            //change the health bar colour based on the player's health remaining
            switch (healthBar.value)
            {
                case > 0.5f:
                    healthBarFillArea.color = Color.green;
                    break;
                case < 0.25f:
                    healthBarFillArea.color = Color.red;
                    break;
                default:
                    healthBarFillArea.color = Color.yellow;
                    break;
            }
        }
    }
}
