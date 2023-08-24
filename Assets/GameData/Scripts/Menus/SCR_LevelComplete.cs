using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SCR_LevelComplete : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI inspectionEnding;
    [SerializeField] private Image fadeToBlack;

    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private GameObject[] enemyDefeatedUI;
    [SerializeField] private GameObject[] enemyScoreUI;

    [SerializeField] private GameObject totalScoreText;
    [SerializeField] private GameObject totalScoreValue;

    [SerializeField] private GameObject timeCompletionText;
    [SerializeField] private GameObject timeCompletionScore;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject returnToMainMenuButton;

    [SerializeField] private float scoreCountUpTime = 2.0f;
    private int[] enemiesDefeated = new int[4];
    private int[] scoreCollected = new int[4];

    public bool isRunning = false;

    private float scoreSoundTimer = 0;
    private float timeBetweenSounds = 0.02f;
    private IEnumerator ShowLevelCompletionUI()
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1f);
        WaitForSecondsRealtime twoSecDelay = new WaitForSecondsRealtime(2f);
        WaitForSecondsRealtime scoreCountUpDelay = new WaitForSecondsRealtime(scoreCountUpTime + 1.0f);

        PSM_MovementStateMachine player = FindObjectOfType<PSM_MovementStateMachine>();
        player.canMove = false;
        GameObject.FindGameObjectWithTag("Player").SetActive(false);

        levelCompleteUI.SetActive(true);
        //UpdateUIWithScore();
        GetScoreData();

        yield return delay;
        for (int i = 0; i < enemyDefeatedUI.Length; i++)
        {
            enemyDefeatedUI[i].SetActive(true);
            yield return twoSecDelay;
            enemyDefeatedUI[i].GetComponent<TextMeshProUGUI>().text += " " + enemiesDefeated[i];

            yield return delay;
            StartCoroutine(CountUpToTarget(enemyScoreUI[i], scoreCollected[i]));
            yield return scoreCountUpDelay;
        }

        timeCompletionText.SetActive(true);
        yield return twoSecDelay;
        timeCompletionText.GetComponent<TextMeshProUGUI>().text = "Time Completed = " + (int)SCR_ScoreTracker.instance.timeCompletion + "s";
        StartCoroutine(CountUpToTarget(timeCompletionScore, SCR_ScoreTracker.instance.TotalTimeScore));
        yield return scoreCountUpDelay;

        totalScoreText.SetActive(true);
        yield return delay;
        totalScoreValue.SetActive(true);
        StartCoroutine(CountUpToTarget(totalScoreValue, SCR_ScoreTracker.instance.OverallScore));
        yield return scoreCountUpDelay;
        continueButton.SetActive(true);
        returnToMainMenuButton.SetActive(true);

        Cursor.visible = true;
        GameManager.gameManager.currentConfidence = SCR_ScoreTracker.instance.OverallScore;
    }

    public IEnumerator DisplayCountdown()
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1f);
        inspectionEnding.gameObject.SetActive(true);
        SCR_AudioManager.instance.Play("SFX_Game_Win");

        isRunning = true;

        yield return delay;

        for (int i = 5; i >= 1; i--)
        {
            inspectionEnding.text = "Inspection ending in " + i + " seconds...";
            yield return delay;
        }
        inspectionEnding.gameObject.SetActive(false);

        fadeToBlack.gameObject.SetActive(true);
        while (fadeToBlack.color.a < 1.0f)
        {
            float alpha = fadeToBlack.color.a + (0.3f * Time.fixedDeltaTime);
            Color colour = fadeToBlack.color;
            colour.a = alpha;

            fadeToBlack.color = colour;

            yield return null;
        }

        StartCoroutine(ShowLevelCompletionUI());
    }

    private void GetScoreData()
    {
        enemiesDefeated[0] = SCR_ScoreTracker.instance.TotalWasabiDefeated;
        enemiesDefeated[1] = SCR_ScoreTracker.instance.TotalRiceDefeated;
        enemiesDefeated[2] = SCR_ScoreTracker.instance.TotalNoriDefeated;
        enemiesDefeated[3] = SCR_ScoreTracker.instance.TotalSalmonDefeated;

        scoreCollected[0] = SCR_ScoreTracker.instance.TotalWasabiScore;
        scoreCollected[1] = SCR_ScoreTracker.instance.TotalRiceScore;
        scoreCollected[2] = SCR_ScoreTracker.instance.TotalNoriScore;
        scoreCollected[3] = SCR_ScoreTracker.instance.TotalSalmonScore;
    }

    private void UpdateUIWithScore()
    {
        enemyDefeatedUI[0].GetComponent<TextMeshProUGUI>().text = "Wasabi Peas Defeated = " + SCR_ScoreTracker.instance.TotalWasabiDefeated;
        enemyDefeatedUI[1].GetComponent<TextMeshProUGUI>().text = "Rice Grains Defeated = " + SCR_ScoreTracker.instance.TotalRiceDefeated;
        enemyDefeatedUI[2].GetComponent<TextMeshProUGUI>().text = "Nori Sheets Defeated = " + SCR_ScoreTracker.instance.TotalNoriDefeated;
        enemyDefeatedUI[3].GetComponent<TextMeshProUGUI>().text = "Salmon Chunks Defeated = " + SCR_ScoreTracker.instance.TotalSalmonDefeated;

        enemyScoreUI[0].GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.TotalWasabiScore.ToString();
        enemyScoreUI[1].GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.TotalRiceScore.ToString();
        enemyScoreUI[2].GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.TotalNoriScore.ToString();
        enemyScoreUI[3].GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.TotalSalmonScore.ToString();

        timeCompletionText.GetComponent<TextMeshProUGUI>().text = "Time Completed = " + (int)SCR_ScoreTracker.instance.timeCompletion + "s";
        timeCompletionScore.GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.TotalTimeScore.ToString();

        totalScoreValue.GetComponent<TextMeshProUGUI>().text = SCR_ScoreTracker.instance.OverallScore.ToString();
    }

    public void ReturnToMainMenu()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
        GameManager.gameManager.ReturnToMainMenu();
    }

    public void LoadNextLevel()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
        GameManager.gameManager.LoadNextLevel();
    }

    /*//Szymon's ground work for the enemy count up
    private IEnumerator CountUpToTarget(TextMeshProUGUI display, float targetVal, float duration, float delay = 0f, string prefix = "")
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        float current = 0;
        while (current < targetVal)
        {
            current += (targetVal / duration / Time.deltaTime);
            current = Mathf.Clamp(current, 0, targetVal);
            display.text = prefix + current;
            yield return null;
        }
    }*/

    private IEnumerator CountUpToTarget(GameObject scoreUIObj, int targetNum)
    {
        scoreUIObj.SetActive(true);
        TextMeshProUGUI score = scoreUIObj.GetComponent<TextMeshProUGUI>();
        SCR_AudioManager.instance.ResetScorePitch();
        scoreSoundTimer = 0;
        timeBetweenSounds = 0.02f;

        float currentNum = 0;
        while (true)
        {
            currentNum = Mathf.Lerp(currentNum, targetNum, Time.deltaTime * scoreCountUpTime);
            score.text = ((int)currentNum).ToString();
            if (targetNum - currentNum < 0.5f)
            {
                score.text = targetNum.ToString();
                break;
            }

            scoreSoundTimer += Time.deltaTime;
            if (scoreSoundTimer >= timeBetweenSounds)
            {
                scoreSoundTimer = 0;
                timeBetweenSounds += 0.01f;
                SCR_AudioManager.instance.PlayScorePingSound();
            }

            yield return null;
        }
    }

}
