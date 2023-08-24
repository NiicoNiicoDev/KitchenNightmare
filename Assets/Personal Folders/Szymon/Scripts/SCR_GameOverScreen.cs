using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCR_GameOverScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image gameOverPanel;
    [SerializeField] private TextMeshProUGUI failureText;

    private int randomNumber;

    public string[] failureMessages;

    private void Awake()
    {
        GetRandomFailureMessage();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetRandomFailureMessage()
    {
        randomNumber = Random.Range(0, failureMessages.Length);
        failureText.SetText(failureMessages[randomNumber]);
    }
}
