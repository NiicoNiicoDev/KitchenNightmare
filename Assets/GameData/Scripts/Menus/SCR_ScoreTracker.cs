using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_ScoreTracker : MonoBehaviour
{
    public enum EnemyType { Wasabi, Rice, Nori, Salmon, RottenWasabi, RottenRice, RottenNori, RottenSalmon, Boss };
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Score awarded per enemy kill")]
    [SerializeField] private int awardedScore_Wasabi = 100;
    [SerializeField] private int awardedScore_Rice = 100;
    [SerializeField] private int awardedScore_Nori = 100;
    [SerializeField] private int awardedScore_Salmon = 100;
    [Header("Rotten Enemies")]
    [SerializeField] private int awardedScore_RottenWasabi = 100;
    [SerializeField] private int awardedScore_RottenRice = 100;
    [SerializeField] private int awardedScore_RottenNori = 100;
    [SerializeField] private int awardedScore_RottenSalmon = 100;

    [Header("Time Completion Variables")]
    [SerializeField] private AnimationCurve completionTimeScale;
    private float flatTimeScore = 100.0f;
    [Tooltip("This value MUST be the same as the final keyframe in Animation Curve!")]
    [SerializeField] private int completionTimeCutOff = 10;

    [Header("Player Stats")]
    [SerializeField] private int currentScore = 0;

    private int[] enemyTypeDefeated = new int[8];

    [HideInInspector] public float timeCompletion { get; private set; } = 0;

    [SerializeField] private GameObject scoreCanvas;

    [Header("Screen-space score text")]
    [SerializeField] private float textFadeSpeed = 1.0f;
    [SerializeField] private float textMoveSpeed = 1.0f;
    [SerializeField] private bool randomUnitCircle = true;

    public static SCR_ScoreTracker instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void ResetCurrentScore()
    {
        for (int i = 0; i < enemyTypeDefeated.Length; i++)
        {
            enemyTypeDefeated[i] = 0;
        }
    }

    public void AddToPlayerScore(EnemyType type, Vector3 enemyPosition)
    {
        int awardedScore = 0;
        switch (type)
        {
            case (EnemyType.Wasabi):
                awardedScore = awardedScore_Wasabi;
                enemyTypeDefeated[0]++;
                break;

            case (EnemyType.Rice):
                awardedScore = awardedScore_Rice;
                enemyTypeDefeated[1]++;
                break;

            case (EnemyType.Nori):
                awardedScore = awardedScore_Nori;
                enemyTypeDefeated[2]++;
                break;

            case (EnemyType.Salmon):
                awardedScore = awardedScore_Salmon;
                enemyTypeDefeated[3]++;
                break;

            //Rotten Enemies

            case (EnemyType.RottenWasabi):
                awardedScore = awardedScore_RottenWasabi;
                enemyTypeDefeated[4]++;
                break;

            case (EnemyType.RottenRice):
                awardedScore = awardedScore_RottenRice;
                enemyTypeDefeated[5]++;
                break;

            case (EnemyType.RottenNori):
                awardedScore = awardedScore_RottenNori;
                enemyTypeDefeated[6]++;
                break;

            case (EnemyType.RottenSalmon):
                awardedScore = awardedScore_RottenSalmon;
                enemyTypeDefeated[7]++;
                break;
        }

        currentScore += awardedScore;

        StartCoroutine(ScreenSpaceScore(awardedScore, enemyPosition));
    }

    private IEnumerator ScreenSpaceScore(int awardedScore, Vector3 enemyPosition)
    {
        GameObject tempTextObj = new GameObject();
        tempTextObj.transform.SetParent(scoreCanvas.transform);

        TextMeshProUGUI newText = tempTextObj.AddComponent<TextMeshProUGUI>();
        newText.verticalAlignment = VerticalAlignmentOptions.Middle;
        newText.horizontalAlignment = HorizontalAlignmentOptions.Center;

        newText.text = "+ " + awardedScore;

        Vector3 direction = Random.insideUnitCircle;
        Vector3 distanceMoved = Vector3.zero;
        if (!randomUnitCircle)
        {
            direction.y = Mathf.Abs(direction.y);
        }

        while (newText.color.a > 0.1f)
        {
            Color newColour = newText.color;
            newColour.a -= textFadeSpeed * Time.fixedDeltaTime;
            newText.color = newColour;

            Vector3 screenSpacePos = Camera.main.WorldToScreenPoint(enemyPosition);
            distanceMoved += direction * textMoveSpeed * Time.fixedDeltaTime;

            newText.rectTransform.position = screenSpacePos + distanceMoved;
            yield return null;
        }

        Destroy(tempTextObj);
    }

    /// <summary>
    /// Adds elapsedTime in seconds to the overall score
    /// </summary>
    /// <param name="elapsedTime"></param>
    public void AddTimeToScore(float elapsedTime)
    {
        timeCompletion = elapsedTime;
    }

    public int TotalWasabiScore
    {
        get {
            int score = enemyTypeDefeated[0] * awardedScore_Wasabi;
            score += enemyTypeDefeated[4] * awardedScore_RottenWasabi;
            return score;
        }
    }

    public int TotalRiceScore
    {
        get
        {
            int score = enemyTypeDefeated[1] * awardedScore_Rice;
            score += enemyTypeDefeated[5] * awardedScore_RottenRice;
            return score;
        }
    }

    public int TotalNoriScore
    {
        get
        {
            int score = enemyTypeDefeated[2] * awardedScore_Nori;
            score += enemyTypeDefeated[6] * awardedScore_RottenNori;
            return score;
        }
    }

    public int TotalSalmonScore
    {
        get
        {
            int score = enemyTypeDefeated[3] * awardedScore_Salmon;
            score += enemyTypeDefeated[7] * awardedScore_RottenSalmon;
            return score;
        }
    }

    public int TotalWasabiDefeated
    {
        get { return enemyTypeDefeated[0] + enemyTypeDefeated[4]; }
    }

    public int TotalRiceDefeated
    {
        get { return enemyTypeDefeated[1] + enemyTypeDefeated[5]; }
    }

    public int TotalNoriDefeated
    {
        get { return enemyTypeDefeated[2] + enemyTypeDefeated[6]; }
    }

    public int TotalSalmonDefeated
    {
        get { return enemyTypeDefeated[3] + enemyTypeDefeated[7]; }
    }

    public int TotalTimeScore
    {
        get
        {
            float minutes = timeCompletion / 60; //Converts to minutes

            if (minutes < 1)
            {
                return (int)Mathf.Round(flatTimeScore * 2);
            }
            else if (minutes > completionTimeCutOff) { return 0; }
            else
            {
                float timeMultiplier = completionTimeScale.Evaluate(minutes);
                return (int)Mathf.Round(flatTimeScore * timeMultiplier);
            }
        }
    }

    public int OverallScore
    {
        get
        {
            return currentScore + TotalTimeScore;
        }
    }

    public bool IsFoodOrderComplete
    {
        get
        {
            SCR_EnemyCounter enemyCounter = FindObjectOfType<SCR_EnemyCounter>();
            return (enemyCounter.bWasabiDefeated && enemyCounter.bRiceDefeated && enemyCounter.bNoriDefeated && enemyCounter.bSalmonDefeated);
        }
    }


}
