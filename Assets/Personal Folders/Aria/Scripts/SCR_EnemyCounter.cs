using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_EnemyCounter : MonoBehaviour
{
    //This script is responsible for keeping track of the number of each enemy in the scene
    //These values are incremented when each enemy is spawned/when their start functions are called, and then decremented upon their defeat

    [SerializeField] int _numberWasabi = 0;
    [SerializeField] int _numberRice = 0;
    [SerializeField] int _numberNori = 0;
    [SerializeField] int _numberSalmon = 0;


    public bool bWasabiDefeated = false;
    public bool bRiceDefeated = false;
    public bool bNoriDefeated = false;
    public bool bSalmonDefeated = false;

    private TextMeshProUGUI wasabiPeaText;
    private TextMeshProUGUI riceGrainText;
    private TextMeshProUGUI noriSheetText;
    private TextMeshProUGUI salmonChunkText;

    private int maxWasabiValue = 0;
    private int maxRiceValue = 0;
    private int maxNoriValue = 0;
    private int maxSalmonValue = 0;

    public int numberWasabiEnemies
    {
        get 
        {
            return _numberWasabi; 
        }
        set
        {
            _numberWasabi = value;
            SetUIText(wasabiPeaText, "Wasabi Peas", maxWasabiValue - value, maxWasabiValue);
            if (_numberWasabi <= 0)
            {
                bWasabiDefeated = true;
                wasabiPeaText.text = "<s>" + wasabiPeaText.text + "</s>";
                SetTextTransparent(wasabiPeaText, 0.3f);
                GameManager.gameManager.SendMessageToSpawner(0);
            }
            //Debug.Log("Number of Wasabi Enemies: " + numberWasabiEnemies);

        }
    }
    public int numberRiceEnemies
    {
        get
        {
            return _numberRice;
        }
        set
        {
            _numberRice = value;
            SetUIText(riceGrainText, "Rice Grains", maxRiceValue - value, maxRiceValue);
            if (_numberRice <= 0)
            {
                bRiceDefeated = true;
                riceGrainText.text = "<s>" + riceGrainText.text + "</s>";
                SetTextTransparent(riceGrainText, 0.3f);
                GameManager.gameManager.SendMessageToSpawner(1);
            }
            //Debug.Log("Number of Rice Enemies: " + numberRiceEnemies);
        }
    }
    public int numberNoriEnemies
    {
        get
        {
            return _numberNori;
        }
        set
        {
            _numberNori = value;
            SetUIText(noriSheetText, "Nori Sheets", maxNoriValue - value, maxNoriValue);
            if (_numberNori <= 0)
            {
                bNoriDefeated = true;
                noriSheetText.text = "<s>" + noriSheetText.text + "</s>";
                SetTextTransparent(noriSheetText, 0.3f);
                GameManager.gameManager.SendMessageToSpawner(2);
            }
            //Debug.Log("Number of Nori Sheet Enemies: " + numberNoriEnemies);
        }
    }
    public int numberSalmonEnemies
    {
        get
        {
            return _numberSalmon;
        }
        set
        {
            _numberSalmon = value;
            SetUIText(salmonChunkText, "Salmon Chunks", maxSalmonValue - value, maxSalmonValue);
            if (_numberSalmon <= 0)
            {
                bSalmonDefeated = true;
                salmonChunkText.text = "<s>" + salmonChunkText.text + "</s>";
                SetTextTransparent(salmonChunkText, 0.3f);
                GameManager.gameManager.SendMessageToSpawner(3);
            }
            //Debug.Log("Number of Salmon Chunk Enemies: " + numberSalmonEnemies);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        /*Debug.Log("Number of Wasabi Enemies: " + numberWasabiEnemies);
        Debug.Log("Number of Rice Enemies: " + numberRiceEnemies);
        Debug.Log("Number of Nori Sheet Enemies: " + numberNoriEnemies);
        Debug.Log("Number of Salmon Chunk Enemies: " + numberSalmonEnemies);*/


        _numberWasabi = GetTotalWaveCount(GameManager.gameManager.fixedWaves[0]);
        _numberRice = GetTotalWaveCount(GameManager.gameManager.fixedWaves[1]);
        _numberNori = GetTotalWaveCount(GameManager.gameManager.fixedWaves[2]);
        _numberSalmon = GetTotalWaveCount(GameManager.gameManager.fixedWaves[3]);

        /*_numberWasabi = 1;
        _numberRice = 1;
        _numberNori = 1;
        _numberSalmon = 1;*/


        maxWasabiValue = _numberWasabi;
        maxRiceValue = _numberRice;
        maxNoriValue = _numberNori;
        maxSalmonValue = _numberSalmon;

        SCR_PauseMenu pauseMenu = FindObjectOfType<PSM_InputHandler>().pauseMenu;

        wasabiPeaText = pauseMenu.wasabiPeaText;
        riceGrainText = pauseMenu.riceGrainText;
        noriSheetText = pauseMenu.noriSheetText;
        salmonChunkText = pauseMenu.salmonChunkText;

        ResetFoodOrder();
    }

    public void ResetFoodOrder()
    {
        SetUIText(wasabiPeaText, "Wasabi Peas", 0, maxWasabiValue);
        SetUIText(riceGrainText, "Rice Grains", 0, maxRiceValue);
        SetUIText(noriSheetText, "Nori Sheets", 0, maxNoriValue);
        SetUIText(salmonChunkText, "Salmon Chunks", 0, maxSalmonValue);

        SetTextTransparent(wasabiPeaText, 1.0f);
        SetTextTransparent(riceGrainText, 1.0f);
        SetTextTransparent(noriSheetText, 1.0f);
        SetTextTransparent(salmonChunkText, 1.0f);

        _numberWasabi = maxWasabiValue;
        _numberRice = maxRiceValue;
        _numberNori = maxNoriValue;
        _numberSalmon = maxSalmonValue;

        bWasabiDefeated = false;
        bRiceDefeated = false;
        bNoriDefeated = false;
        bSalmonDefeated = false;
    }

    int GetTotalWaveCount(SO_FixedEnemyWave wave)
    {
        int enemyCount = 0;
        for (int i = 0; i < wave.enemyWaves.Count; i++)
        {
            enemyCount += wave.enemyWaves[i].enemiesInWave.Length;
        }

        return enemyCount;
    }

    private void SetUIText(TextMeshProUGUI enemyStat, string enemyName, int currentValue, int maxValue)
    {
        enemyStat.text = enemyName + " = (" + currentValue + " / " + maxValue + ")";
    }

    private void SetTextTransparent(TextMeshProUGUI enemyText, float value)
    {
        Color colour = enemyText.color;
        colour.a = value;
        enemyText.color = colour;
    }

}
