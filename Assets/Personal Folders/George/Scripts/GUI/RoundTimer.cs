using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundTimer : MonoBehaviour
{
    private bool bRoundActive = false;
    [SerializeField] TMP_Text timerText;
    [SerializeField] float maxRoundTime;
    public float timer;
    private float completionTime = 0;

    public float GetCompletionTime { get { return completionTime; } }

    private void Awake()
    {
        timer = 0f;
        timerText.text = "Waiting for round to start";


    }

    // Update is called once per frame
    void Update()
    {
        if (bRoundActive)
        {
            timer += Time.deltaTime;

            completionTime += Time.deltaTime;
            timerText.text = "Time: " + FormatTime();

            if (timer >= maxRoundTime)
            {
                timerText.color = Color.red;
            }
        }
        
    }

    public void OnRoundStart()
    {
        bRoundActive = true;
        timer = 0f;
    }

    public void OnRoundEnd()
    {
        bRoundActive = false;

        timer = 0;
        timerText.text = "Waiting for round to start";
    }

    string FormatTime()
    {
        int minutes = (int)timer / 60;
        int seconds = (int)timer - (60 * minutes);

        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}
