using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SCR_VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Toggle muteToggle;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;

    private void Awake()
    {
        masterVolumeSlider.value = SCR_AudioManager.instance.currentSliderValues[0];
        SFXVolumeSlider.value = SCR_AudioManager.instance.currentSliderValues[1];
        musicVolumeSlider.value = SCR_AudioManager.instance.currentSliderValues[2];
        muteToggle.isOn = SCR_AudioManager.instance.bVolumeMuted;
    }
    public void SetMuteToggle()
    {
        SCR_AudioManager.instance.bVolumeMuted = muteToggle.isOn;
        if (muteToggle.isOn)
        {
            mixer.SetFloat("MasterVol", Mathf.Log10(0.0001f) * 20);
        }
        else if (!muteToggle.isOn)
        {
            mixer.SetFloat("MasterVol", Mathf.Log10(masterVolumeSlider.value) * 20);
        }
    }

    public void SetMasterVolume (float sliderValue)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
        SCR_AudioManager.instance.currentSliderValues[0] = masterVolumeSlider.value;
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue * 20));
        SCR_AudioManager.instance.currentSliderValues[1] = SFXVolumeSlider.value;
    }

    public void SetMusicVolume (float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        SCR_AudioManager.instance.currentSliderValues[2] = musicVolumeSlider.value;
    }
}
