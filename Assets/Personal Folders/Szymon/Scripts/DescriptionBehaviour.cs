using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionBehaviour : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private string startingText;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        startingText = textMesh.text;
        textMesh.text = startingText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetDefaultText()
    {
        textMesh.text = startingText;
    }
    public void SetNewGame()
    {
        textMesh.text = "New Game: \r\n Start a new game";
    }

    public void SetLoadGame()
    {
        textMesh.text = "Load Game: \r\n Load previous game";
    }

    public void SetOptions()
    {
        textMesh.text = "Options: \r\n Open options menu that will allow you to edit specifications of this game";
    }

    public void SetExit()
    {
        textMesh.text = "Exit: \r\n Exit the game";
    }

    public void SetAudio()
    {
        textMesh.text = "Audio: \r\n Set your audio preferences";
    }

    public void SetControls()
    {
        textMesh.text = "Controls: \r\n Set your controls preferences";
    }

    public void SetVideo()
    {
        textMesh.text = "Video: \r\n Set your video preferences";
    }

    public void SetBack()
    {
        textMesh.text = "Back: \r\n Go back to the Main Menu";
    }
}
