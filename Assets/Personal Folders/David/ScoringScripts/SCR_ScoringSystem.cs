using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//controls the scoring system for the player during recipe compiling and boss fights
public class SCR_ScoringSystem : MonoBehaviour
{
    #region EDITABLE SCORING VALUES
    //the highest score that can be achieved for the recipe
    [Header(header:"For variable descriptions, see script comments")]
    public int maximumScore = 1;

    //how much the score decreases by every x seconds (x = decrease rate) if the maximum score time has elapsed
    [SerializeField] private int scoreDecrease = 1;

    //the lowest possible score that can be achieved for the recipe
    [SerializeField] private int minimumScore = 1;

    //how quickly the score drops once the maximum score time has elapsed
    [SerializeField] private float decreaseRate = 1f;

    //the time required for the player to achieve the maximum score. Past this time, the score achieved will begin to drop
    [SerializeField] private float maximumScoreTime = 1f;
    #endregion

    //reference to the timer text box
    [SerializeField] private GameObject timerTextBox;

    //reference to the score achieved text
    [SerializeField] private GameObject scoreAchievedTextBox;

    [SerializeField] private SCR_EnemyCounter enemyCount;

    //reference to the timer text
    private TextMeshProUGUI timerText;

    //times how long the recipe took to compile
    private float timer = 0f;

    private WaitForSeconds scoreDecreaseRate;

    //controls when the timer should increase
    private bool runTimer = false;

    //holds the live score achieved by the player
    private int scoreAchieved = 0;

    //holds details about when the score is decreasing
    private bool decreasingScore = false;

    // Start is called before the first frame update
    void Start()
    {
        //set the score achieved initially to the maximum score
        scoreAchieved = maximumScore;

        //initialise the decrease rate
        scoreDecreaseRate = new WaitForSeconds(decreaseRate);

        //initialise timer text and hide text box
        timerText = timerTextBox.GetComponent<TextMeshProUGUI>();
        timerTextBox.SetActive(false);

        //hide the score text
        scoreAchievedTextBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if the timer should be running
        if (runTimer)
        {
            //then increase it as per game time
            timer += Time.deltaTime;

            timerText.text = timer.ToString("F1") + "s";

            //if the maximum score time has elapsed and the score isn't being decreased
            if (!decreasingScore && timer > maximumScoreTime)
            {
                //then set the decreasing score bool to true so the co-routine isn't called every frame
                decreasingScore = true;

                //begin decreasing the player's score
                StartCoroutine("DecreaseScore");
            }

            if(enemyCount.bNoriDefeated && enemyCount.bRiceDefeated & enemyCount.bWasabiDefeated && enemyCount.bSalmonDefeated)
            {
                FinishedRecipe();
            }
        }
    }

    //controls the score decreasing behaviour
    IEnumerator DecreaseScore()
    {
        //while the timer is running and the player hasn't dropped below the minimum round score
        while (runTimer && scoreAchieved > minimumScore)
        {
            //decrease the score at the rate set in the inspector
            scoreAchieved -= scoreDecrease;

            //if the new score has dropped below the minimum score, set the score to the minimum score
            if(scoreAchieved < minimumScore)
            {
                scoreAchieved = minimumScore;
            }

            //wait for x seconds (x = decrease rate)
            yield return scoreDecreaseRate;
        }

        //set the decreasing score bool to false so the co-routine can be called in the future if needed
        decreasingScore = false;
    }

    //called when the recipe compiling process (or a boss fight) has begun
    public void StartCompilingRecipe()
    {
        //reset the timer and then run it
        timer = 0f;
        timerTextBox.SetActive(true);
        timerText.text = timer.ToString("F1") + "s";
        runTimer = true;
    }

    //pauses timer for instances such as deaths and cutscenes
    public void PauseTimer()
    {
        runTimer = false;
    }

    //continues timer after it was paused
    public void ContinueTimer()
    {
        //if timer is at 0, a recipe was not being compiled so don't start the timer
        if (timer > 0f)
        {
            runTimer = true;
        }
    }

    /*public void EnemyKilled(SCR_EnemyStats killedEnemy)
    {
        enemiesToKill.Remove(killedEnemy);

        if(enemiesToKill.Count <= 0)
        {
            FinishedRecipe();
        }
    }*/

    //called when the recipe has been completed (or the boss fight has ended)
    private void FinishedRecipe()
    {
        //stop the timer
        runTimer = false;

        timer = 0f;

        //hide the text box
        timerTextBox.SetActive(false);

        //begin displaying the score achieved
        StartCoroutine("DisplayScoreAchieved");

        //increase the player's score by the score they earnt
        GameManager.gameManager.IncreaseScore(scoreAchieved);

        GameManager.gameManager.bRecipeTriggered = false;
    }

    IEnumerator DisplayScoreAchieved()
    {
        //display the score achieved
        scoreAchievedTextBox.SetActive(true);

        scoreAchievedTextBox.GetComponent<TextMeshProUGUI>().text = "You won " + scoreAchieved + " points!";
        
        //wait 2 seconds
        yield return new WaitForSeconds(2f);

        //hide the score achieved as it may distract the player
        scoreAchievedTextBox.SetActive(false);
    }
}
