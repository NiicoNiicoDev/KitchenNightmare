using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_GameOverMenuCalls : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SCR_SceneManager.instance.LoadScene(0);
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }

    public void RestartLevel()
    {
        GameManager.gameManager.ResetLevel();
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }
}
