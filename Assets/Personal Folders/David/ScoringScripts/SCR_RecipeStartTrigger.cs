using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//starts the recipe compiling process
//Make sure the trigger is located BEFORE any enemies that are part of the recipe
public class SCR_RecipeStartTrigger : MonoBehaviour
{
    //reference to the scoring system for the current recipe
    [Header(header:"Ensure the correct scoring system is \nselected for this")]
    [SerializeField] private SCR_ScoringSystem scoringSystem;

    private void OnTriggerEnter(Collider other)
    {
        //if the player has collided with the trigger
        if (other.CompareTag("Player") && !GameManager.gameManager.bRecipeTriggered)
        {
            GameManager.gameManager.AddHighestScore(scoringSystem.maximumScore);

            //start the scoring timer
            scoringSystem.StartCompilingRecipe();

            GameManager.gameManager.bRecipeTriggered = true;

            Destroy(gameObject);
        }
    }
}
