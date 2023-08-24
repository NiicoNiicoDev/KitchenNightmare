using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Reference for debugging/disable before building or OnGUI elements will be visible in the built version
    public static bool isDebugMode = false;
    //referenve to game manager for easier access from other scripts
    public static GameManager gameManager { get; private set; }

    public bool bossDefeated;

    public List<GameObject> unlockedWeapons;
    
    //holds how much confidence the player currently has
    public int currentConfidence = 0;
    
    //holds details about whether a recipe is being created
    [HideInInspector] public bool bRecipeTriggered = false;

    //holds the current player's score
    private int playerScore = 0;

    //holds the score earnt in the current round
    private int roundScore = 0;

    private int highestRoundScore = 0;

    private float confidenceEarnt = 0f;

    private SCR_OpenStore storeScript;

    //holds the score text (may be later changed to an icon)
    private TextMeshProUGUI scoreText;

    private TextMeshProUGUI confidenceText;

    [SerializeField] private GameObject waveSpawner;
    public SO_FixedEnemyWave[] fixedWaves;

    [SerializeField] private GameObject playerHUD;

    public bool bPlayerDead = false;
    public bool bLevelComplete = false;

    private int spawnersInLevel = 0;
    private List<SCR_EnemySpawner> fixedEnemySpawnersInLevel = new List<SCR_EnemySpawner>();

    [SerializeField] private float timeSinceLastKill = 0.0f;

    public const int THREE_MINUTES = 180;

    public bool bRoundStarted = false;

    private bool bIntroBossMusicPlaying = false;
    private bool bMainBossLoopPlaying = false;
    private bool bBossDefeatedPlaying = false;

    public int previousSeed = -1;

    [SerializeField] private bool doNotDestroy = true;

    private void Awake()
    {
        //don't destroy game manager on scene loading
        if(gameManager == null)
        {
            gameManager = this;
            if (doNotDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        } 
        else if(gameManager != this) //but if one already exist, destroy the duplicate as it may create issues
        {
            Destroy(gameObject);
        }

        if(gameManager == this)
        {
            /*foreach(GameObject weapon in unlockedWeapons)
            {
                weapon.GetComponent<SCR_Weapon>().upgradeLevel = 1;
            }*/
        }

        //call the on scene loaded function to implement any initialisation
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*roundScore = 0;

        scoreText = GameObject.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
        confidenceText = GameObject.Find("ConfidenceText").GetComponent<TextMeshProUGUI>();

        scoreText.text = "Score: " + playerScore;
        confidenceText.text = "Confidence: " + currentConfidence;

        bRecipeTriggered = false;

        storeScript = GameObject.Find("Main Camera").GetComponent<SCR_OpenStore>();*/
    }

    public void DistributeSpawners()
    {
        SCR_ProceduralGeneration gen = FindObjectOfType<SCR_ProceduralGeneration>();
        GameObject[] rooms = gen.GetDungeonRooms;

        Random.InitState(gen.seed);

        List<int> rand = new List<int>();
        rand.Add(Random.Range(2, rooms.Length));

        for (int i = 1; i < fixedWaves.Length; i++)
        {
            int num = Random.Range(2, rooms.Length);
            while (rand.Contains(num))
            {
                num = Random.Range(2, rooms.Length);
            }
            rand.Add(num);
        }

        for (int j = 2, k = 0; j < rooms.Length; j++)
        {
            SCR_EnemySpawner spawner = Instantiate(waveSpawner, rooms[j].transform).GetComponent<SCR_EnemySpawner>();

            BoxCollider trigger = spawner.GetComponent<BoxCollider>();
            trigger.size = rooms[j].GetComponent<SCR_RoomSettings>().waveTriggerSize;

            spawner.areaSizeX = trigger.size.x / 2;
            spawner.areaSizeZ = trigger.size.z / 2;
            if (rand.Contains(j))
            {
                spawner.enemyWaveSO = fixedWaves[k];
                k++;
                fixedEnemySpawnersInLevel.Add(spawner);
            }
            else
            {
                spawner.enemyWaveSO = null;
            }
        }
        #region Editor_Seed_Display
#if (UNITY_EDITOR)
        GameObject textObj = new GameObject("Current Seed");
        textObj.transform.SetParent(playerHUD.transform);

        TextMeshProUGUI seed = textObj.AddComponent<TextMeshProUGUI>();
        seed.text = "Seed = " + gen.seed.ToString();

        seed.rectTransform.anchorMin = Vector2.up;
        seed.rectTransform.anchorMax = Vector2.up;
        seed.rectTransform.pivot = Vector2.up;

        seed.rectTransform.anchoredPosition = Vector2.zero;
#endif
        #endregion

        spawnersInLevel = rooms.Length - 2;

        FindObjectOfType<SCR_CooldownSpawner>().FindSpawnPositions();
    }

    public void AddHighestScore(int highestScore)
    {
        highestRoundScore += highestScore;
    }

    //called to increase the player's score
    public void IncreaseScore(int scoreIncrease)
    {
        //add the score increase to the player's score
        playerScore += scoreIncrease;
        roundScore+= scoreIncrease;

        //then update the score text
        scoreText.text = "Score: " + playerScore;
    }

    void SetConfidence()
    {
        confidenceEarnt = ((float)roundScore / (float)highestRoundScore) * 100f;

        Debug.Log("New confidence: " + confidenceEarnt);

        currentConfidence += (int)confidenceEarnt;

        confidenceText.text = "Confidence: " + currentConfidence;
    }

    public void DecreaseConfidence(int confidenceReduction)
    {
        currentConfidence -= confidenceReduction;

        confidenceText.text = "Confidence: " + currentConfidence;
    }

    private void Update()
    {
        if(bossDefeated)
        {
            bossDefeated= false;

            SetConfidence();

            storeScript.OpenStore();
        }

        if (bRoundStarted)
        {
            timeSinceLastKill += Time.deltaTime;

            if (timeSinceLastKill >= THREE_MINUTES)
            {
                CheckFoodOrderCompletion();
            }
        }
    }

    public void ResetTimeSinceLastKill()
    {
        timeSinceLastKill = 0.0f;
    }

    public void LoadNextRound(string nextSceneName)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void ResetLevel()
    {
        Time.timeScale = 1;
        SCR_SafeRoom safeRoom = FindObjectOfType<SCR_SafeRoom>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Animator animator = player.GetComponent<Animator>();

        safeRoom.ResetSafeRoom();
        safeRoom.RecallPlayer();

        animator.SetBool("isDead", false);
        animator.SetBool("isMoving", false);

        Cursor.visible = false;

        GameObject.FindGameObjectWithTag("GameOverCanvas").SetActive(false);

        player.GetComponent<SCR_PlayerStats>().ResetHealth();

        foreach (SCR_EnemySpawner spawner in FindObjectsOfType<SCR_EnemySpawner>())
        {
            spawner.ResetSpawner();
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy.name == "P_AI_SushiRollBoss") { continue; }
            Destroy(enemy);
        }

        player.GetComponent<SCR_WeaponHandler>().ResetWeaponCooldowns();

        FindObjectOfType<SCR_EnemyCounter>().ResetFoodOrder();

        FindObjectOfType<SCR_ScoreTracker>().ResetCurrentScore();

        FindObjectOfType<RoundTimer>().OnRoundEnd();

        FindObjectOfType<SCR_AI_SushiRoll>().ResetBoss();

        bPlayerDead = false;
        timeSinceLastKill = 0.0f;
        bRoundStarted = false;
    }



    public void LevelEnded()
    {
        RoundTimer timer = FindObjectOfType<RoundTimer>();
        SCR_ScoreTracker.instance.AddTimeToScore(timer.GetCompletionTime);
        FindObjectOfType<SCR_PauseMenu>().DisableQuickAccessMenu();
        bLevelComplete = true;

        SCR_LevelComplete levelComplete = FindObjectOfType<SCR_LevelComplete>();
        if (!levelComplete.isRunning)
        {
            levelComplete.StartCoroutine(levelComplete.DisplayCountdown());
        }

        foreach (SCR_EnemySpawner spawner in FindObjectsOfType<SCR_EnemySpawner>())
        {
            Destroy(spawner);
        }

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        FindObjectOfType<SCR_PlayerStats>().canTakeDamage = false;


        if (playerHUD == null)
        {
            playerHUD = GameObject.FindGameObjectWithTag("HUD");
        }
        playerHUD.SetActive(false); 
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        bLevelComplete = false;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    public void DecrementSpawnersInLevel()
    {
        spawnersInLevel--;

        if (spawnersInLevel <= 0)
        {
            CheckFoodOrderCompletion();
        }
    }

    private void CheckFoodOrderCompletion()
    {
        if (!SCR_ScoreTracker.instance.IsFoodOrderComplete)
        {
            foreach (SCR_EnemySpawner spawner in fixedEnemySpawnersInLevel)
            {
                if (!spawner.bEnemyTypeDefeated)
                {
                    spawner.ResetSpawner();
                }
            }
        }
        timeSinceLastKill = 0.0f;
    }

    public void SendMessageToSpawner(int fixedSpawnerIndex)
    {
        fixedEnemySpawnersInLevel[fixedSpawnerIndex].bEnemyTypeDefeated = true;
    }

    public void PlayBossIntroMusic()
    {
        if (!bIntroBossMusicPlaying)
        {
            SCR_AudioManager.instance.PlayMusic("Boss fight - Whole");
            SCR_AudioManager.instance.TransitionToMainBossLoop();
            bIntroBossMusicPlaying = true;
        }
    }

    public void PlayBossMainLoopMusic()
    {
        if (!bMainBossLoopPlaying)
        {
            SCR_AudioManager.instance.TransitionToMainBossLoop();
            bMainBossLoopPlaying = true;
        }
    }

    public void PlayBossDefeatedMusic()
    {
        if (!bBossDefeatedPlaying)
        {
            SCR_AudioManager.instance.StopAllCoroutines();
            SCR_AudioManager.instance.PlayMusic("Boss fight - End");
            bBossDefeatedPlaying = true;
        }
    }
}

