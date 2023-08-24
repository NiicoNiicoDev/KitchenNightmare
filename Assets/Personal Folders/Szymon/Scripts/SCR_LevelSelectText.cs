using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LevelSelectText : MonoBehaviour
{
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private GameObject gameText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenGameText()
    {
        gameText.SetActive(true);
        tutorialText.SetActive(false);
    }

    public void OpenTutorialText()
    {
        gameText.SetActive(false);
        tutorialText.SetActive(true);
    }
}
