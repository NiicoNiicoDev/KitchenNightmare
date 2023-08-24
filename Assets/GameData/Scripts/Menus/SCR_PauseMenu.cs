using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_PauseMenu : MonoBehaviour
{
    [SerializeField] private AnimationClip[] animationClips;
    [SerializeField] private Animation quickAccessAnimation;

    [SerializeField] private Animation pauseMenuAnimation;


    public TextMeshProUGUI wasabiPeaText;
    public TextMeshProUGUI riceGrainText;
    public TextMeshProUGUI noriSheetText;
    public TextMeshProUGUI salmonChunkText;

    [SerializeField] private GameObject quickAccessObj;
    [SerializeField] private GameObject pauseMenuObj;

    bool quickAccessActive = false;
    bool pauseMenuActive = false;

    public bool collectedOrder = false;

    [SerializeField] private GameObject HUD;


    public void ToggleQuickAccessMenu()
    {
        if (this == null) { return; }

        if (!collectedOrder || pauseMenuObj.activeSelf) { return; }
        if (GameManager.gameManager != null && (GameManager.gameManager.bPlayerDead || GameManager.gameManager.bLevelComplete)) { return; }

        if (!quickAccessObj.activeSelf)
        {
            quickAccessAnimation.clip = animationClips[0];
            quickAccessObj.SetActive(true);

            if (!quickAccessAnimation.isPlaying)
            {
                quickAccessAnimation.Play();
                SCR_AudioManager.instance.Play("SFX_Button");
            }
        }
        else if (quickAccessActive)
        {
            quickAccessAnimation.clip = animationClips[1];

            if (!quickAccessAnimation.isPlaying)
            {
                quickAccessAnimation.Play();
            }
        }

    }

    public void TogglePauseMenu()
    {
        if (this == null) { return; }

        if (quickAccessObj.activeSelf) { return; }
        if (GameManager.gameManager != null && (GameManager.gameManager.bPlayerDead || GameManager.gameManager.bLevelComplete)) { return; }

        if (!pauseMenuObj.activeSelf)
        {
            Cursor.visible = true;
            pauseMenuAnimation.clip = animationClips[0];
            pauseMenuObj.SetActive(true);

            HUD.SetActive(false);

            if (!pauseMenuAnimation.isPlaying)
            {
                pauseMenuAnimation.Play();
                SCR_AudioManager.instance.Play("SFX_Button");
            }
        }
        else if (pauseMenuActive)
        {
            Cursor.visible = false;
            Time.timeScale = 1;
            pauseMenuAnimation.clip = animationClips[1];
            HUD.SetActive(true);

            if (!pauseMenuAnimation.isPlaying)
            {
                pauseMenuAnimation.Play();
            }
        }
    }



    public void DisableQuickAccessMenu()
    {
        quickAccessActive = false;
        quickAccessObj.SetActive(false);

        pauseMenuActive = false;
        pauseMenuObj.SetActive(false);
    }

    public void AnimationDone()
    {
        quickAccessActive = true;
        pauseMenuActive = true;

        if (pauseMenuObj.activeSelf)
        {
            Time.timeScale = 0;
        }
    }

    public void ReturnToMainMenu()
    {
        //GameManager.gameManager.ReturnToMainMenu();
        SCR_SceneManager.instance.LoadScene(0);
    }

    public void PlayInteractSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_UI");
    }
}
