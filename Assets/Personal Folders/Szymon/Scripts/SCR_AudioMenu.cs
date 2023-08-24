using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCR_AudioMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private TextMeshProUGUI masterSliderTxt;
    [SerializeField] private TextMeshProUGUI sfxSliderTxt;
    [SerializeField] private TextMeshProUGUI musicSliderTxt;

    [SerializeField] private Toggle muteToggle;

    private float masterSliderVal;
    private float sfxSliderVal;
    private float musicSliderVal;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        masterSliderVal = masterSlider.value * 100;
        sfxSliderVal = sfxSlider.value * 100;
        musicSliderVal = musicSlider.value * 100;

        masterSliderTxt.SetText(masterSliderVal.ToString("0"));
        sfxSliderTxt.SetText(sfxSliderVal.ToString("0"));
        musicSliderTxt.SetText(musicSliderVal.ToString("0"));
    }
}
