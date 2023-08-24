using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_MainMenuAnim : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private Animator anim;
    [SerializeField] private Animator doorAnim;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject exitMenu;

    private int currentLevel = 2; //Current Level Could potentially be loaded from memory in the future and is used in StartGame()

    void Start()
    {
        //anim = FindObjectOfType<Animator>();
        SCR_AudioManager.instance.PlayMusic("Main Theme");
        Time.timeScale = 1;
        Cursor.visible = true;
    }

    public void PlayLevelSelectSound()
    {
        SCR_AudioManager.instance.Play("SFX_Game_Select");
    }

    private void Awake()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayButtonPressSound()
    {
        FindObjectOfType<SCR_AudioManager>().Play("SFX_Button");
    }
    public void PlayQuitAnim()
    {
        doorAnim.SetTrigger("Quit");
        anim.SetTrigger("Quit");
        exitMenu.SetActive(false);
        Invoke("ExitGame", 1);
    }
    public void PlayCameraAnim()
    {
        anim.SetTrigger("Camera");
    }

    public void PlayCameraAnimReverse()
    {
        anim.SetTrigger("CameraReverse");
    }

    public void PlayCameraCreditsAnim()
    {
        anim.SetTrigger("CameraCredits");
    }

    public void PlayCameraCreditsAnimReverse()
    {
        anim.SetTrigger("CameraCreditsReverse");
    }

    public void PlayCameraStartAnim()
    {
        anim.SetTrigger("CameraStart");
    }

    public void PlayCameraStartAnimReverse()
    {
        anim.SetTrigger("CameraStartReverse");
    }

    public void PlayCameraExitAnim()
    {
        anim.SetTrigger("CameraExit");
    }

    public void PlayCameraExitAnimReverse()
    {
        anim.SetTrigger("CameraExitReverse");
    }

    private void SetMenuInactive()
    {
        mainMenu.SetActive(false);
    }

    private void SetOptionsActive()
    {
        options.SetActive(true);
    }

    private void SetMenuActive()
    {
        mainMenu.SetActive(true);
    }

    private void SetOptionsInactive()
    {
        options.SetActive(false);
    }

    private void SetCreditsActive()
    {
        creditsMenu.SetActive(true);
        FindObjectOfType<SCR_AudioManager>().PauseSound("Main Theme");
    }

    private void SetCreditsInactive()
    {
        creditsMenu.SetActive(false);
        FindObjectOfType<SCR_AudioManager>().ResumeSound("Main Theme");
    }

    private void SetStartActive()
    {
        startMenu.SetActive(true);
    }

    private void SetStartInactive()
    {
        startMenu.SetActive(false);
    }

    private void SetExitActive()
    {
        exitMenu.SetActive(true);
    }

    private void SetExitInactive()
    {
        exitMenu.SetActive(false);
    }

    public void SetMenuInactiveInvoke()
    {
        Invoke("SetMenuInactive", 0.4f);
    }
    public void SetMenuActiveInvoke()
    {
        Invoke("SetMenuActive", 0.4f);
    }
    public void SetOptionsActiveInvoke()
    {
        Invoke("SetOptionsActive", 0.1f);
    }
    public void SetOptionsInactiveInvoke()
    {
        Invoke("SetOptionsInactive", 0.1f);
    }
    public void SetCreditsActiveInvoke()
    {
        Invoke("SetCreditsActive", 0.1f);
    }
    public void SetCreditsInactiveInvoke()
    {
        Invoke("SetCreditsInactive", 0.1f);
    }
    public void SetStartActiveInvoke()
    {
        Invoke("SetStartActive", 0.1f);
    }
    public void SetStartInactiveInvoke()
    {
        Invoke("SetStartInactive", 0.1f);
    }
    public void SetExitActiveInvoke()
    {
        Invoke("SetExitActive", 0.1f);
    }
    public void SetExitInactiveInvoke()
    {
        Invoke("SetExitInactive", 0.1f);
    }
    
    public void StartGame()
    {
        //SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Single);
        //StartCoroutine(LoadAsync(currentLevel));

        SCR_SceneManager.instance.LoadScene(currentLevel);
    }

    public void LoadLevel1()
    {
        //SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        //StartCoroutine(LoadAsync(1));

        SCR_SceneManager.instance.LoadScene(1);
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);

        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void LoadLevel2()
    {

    }

    public void LoadLevel3()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayButtonSound()
    {
        FindObjectOfType<SCR_AudioManager>().Play("SFX_Game_UI");
    }
}
