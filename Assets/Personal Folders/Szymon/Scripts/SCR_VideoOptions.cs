using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_VideoOptions : MonoBehaviour
{

    Resolution[] resolutions;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionNum = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionNum = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionNum;
        resolutionDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetQuality(int qualityNum)
    {
        QualitySettings.SetQualityLevel(qualityNum);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionNum)
    {
        Resolution resolution = resolutions[resolutionNum];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
