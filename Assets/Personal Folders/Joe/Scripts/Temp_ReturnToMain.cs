using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Temp_ReturnToMain : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
