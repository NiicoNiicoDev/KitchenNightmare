using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class SCR_SceneManager : MonoBehaviour
{
    [SerializeField] private  VideoPlayer loadingScreen;
    [SerializeField] private float loadingTime = 3.0f;
    private WaitForSecondsRealtime delay;

    [SerializeField] private RawImage videoImage;
    [SerializeField] private float fadeOutTime = 1.0f;
    public static SCR_SceneManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            delay = new WaitForSecondsRealtime(loadingTime);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        loadingScreen.gameObject.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        async.allowSceneActivation = false;

        yield return delay;

        while (loadingScreen.time % 2 > 0 || async.progress < 0.9f)
        {
            yield return null;
        }

        async.allowSceneActivation = true;
        yield return null;
        loadingScreen.Pause();

        while (videoImage.color.a > 0.1f)
        {
            Color colour = videoImage.color;
            colour.a -= Time.fixedDeltaTime * fadeOutTime;
            videoImage.color = colour;
            yield return null;
        }
        videoImage.color = Color.white;
        loadingScreen.gameObject.SetActive(false);
    }
}
