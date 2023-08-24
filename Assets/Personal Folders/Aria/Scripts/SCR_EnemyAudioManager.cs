using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//This script attaches to each enemy and controls the sounds that they will play
[RequireComponent(typeof(AudioSource))]
public class SCR_EnemyAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] _audioClips;
    public AudioClip[] AudioClips
    {
        get
        {
            return _audioClips;
        }
    }

    bool bUseManager = true;

    private void Start()
    {
        if (AudioClips.Length == 0 || audioSource == null)
        {
            bUseManager = false;
            this.enabled = false;
        }
    }

    public void PlaySound(AudioClip clip, bool loop)
    {
        if (bUseManager)
        {
            try
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.loop = loop;
                audioSource.Play();
            }
            catch(IndexOutOfRangeException e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void PlayRandomSound()
    {
        if(bUseManager)
        {
            audioSource.Stop();
            int random = UnityEngine.Random.Range(0, AudioClips.Length);
            audioSource.PlayOneShot(AudioClips[random]);
        }
    }

    
}
