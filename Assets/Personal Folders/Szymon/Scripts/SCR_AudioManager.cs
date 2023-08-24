using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Random = UnityEngine.Random;

public class SCR_AudioManager : MonoBehaviour
{
    List<Sound[]> soundArrays = new List<Sound[]>();

    public Sound[] sfx;
    public Sound[] restaurantAmb;
    public Sound[] music;
    public Sound[] bossBarks;
    public Sound[] peasBarks;
    public Sound[] noriBarks;
    public Sound[] salmonBarks;
    public Sound[] riceBarks;

    private Sound currentSound;

    public static SCR_AudioManager instance;

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup walkingMixerGroup;
    [SerializeField] private AudioMixerGroup scorePingGroup;
    public AudioMixerGroup sfxMixerGroup;

    private Sound currentMusicTrack;

    public enum MixerGroup { SFX, Music }
    private AudioMixerGroup[] mixerGroups = new AudioMixerGroup[2];

    public float[] currentSliderValues = new float[3] { 1.0f, 1.0f, 1.0f };
    public bool bVolumeMuted = false;


    private Coroutine walkingRoutine;
    private Sound walkSound;
    private Sound scoreSound;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        AddSoundArraysToList();
        AddComponentsToSounds();

        mixerGroups[0] = sfxMixerGroup;
        mixerGroups[1] = musicMixerGroup;

        #region Redundant Code
        //CODE ON LINES 67 THROUGH 154 IS NOW REDUNDANT REPLACED WITH CODE ON LINES 42 THROUGH 63

        /*foreach (Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in restaurantAmb)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = musicMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in bossBarks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in peasBarks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in noriBarks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in salmonBarks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }

        foreach (Sound s in riceBarks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = sfxMixerGroup;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            AudioSourceCount++;
        }*/
        #endregion
    }

    void AddSoundArraysToList()
    {
        soundArrays.Add(sfx);
        soundArrays.Add(restaurantAmb);
        soundArrays.Add(music);
        soundArrays.Add(bossBarks);
        soundArrays.Add(peasBarks);
        soundArrays.Add(noriBarks);
        soundArrays.Add(salmonBarks);
        soundArrays.Add(riceBarks);
    }

    void AddComponentsToSounds()
    {
        foreach (Sound[] array in soundArrays)
        {
            foreach (Sound s in array)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                //s.source.outputAudioMixerGroup = sfxMixerGroup;
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    public void ResetScorePitch()
    {
        scorePingGroup.audioMixer.SetFloat("ScorePing", 1.0f);
    }
    public void PlayScorePingSound()
    {
        if (scoreSound == null)
        {
            scoreSound = GetSound("SFX_ScorePing");
            scoreSound.source.outputAudioMixerGroup = scorePingGroup;
        }

        float newPitch;
        scorePingGroup.audioMixer.GetFloat("ScorePing", out newPitch);
        newPitch += 0.1f;

        scorePingGroup.audioMixer.SetFloat("ScorePing", newPitch);
        scoreSound.source.Play();
            
    }

    public void TransitionToMainBossLoop()
    {
        StartCoroutine(WaitForEndOfIntro());
    }
    private IEnumerator WaitForEndOfIntro()
    {
        while (currentMusicTrack.source.isPlaying)
        {
            yield return null;
        }

        PlayMusic("Boss fight - Main loop");
    }

    public void SetPlayerWalking(bool playerWalking)
    {
        if (playerWalking)
        {
            walkingRoutine = StartCoroutine(PlayWalkingSounds());
        }
        else if (walkingRoutine != null)
        {
            StopCoroutine(walkingRoutine);
            GetSound("SFX_Char_Step_1").source.Stop();
        }
    }

    private IEnumerator PlayWalkingSounds()
    {
        if (walkSound == null)
        {
            walkSound = GetSound("SFX_Char_Step_1");
            walkSound.source.outputAudioMixerGroup = walkingMixerGroup;
        }

        WaitForSeconds delay = new WaitForSeconds(walkSound.source.clip.length / 2);
        while (true)
        {
            float randPitch = Random.Range(0.5f, 1.0f);
            walkingMixerGroup.audioMixer.SetFloat("WalkingPitch", randPitch);
            walkSound.source.Play();
            yield return delay;
        }
    }

    public void PlayMusic(string name, MixerGroup mixer = MixerGroup.Music)
    {
        if (currentMusicTrack != null)
        {
            currentMusicTrack.source.Stop();
            currentMusicTrack = null;
        }

        foreach (Sound[] soundArray in soundArrays)
        {
            if (currentMusicTrack == null)
            {
                currentMusicTrack = Array.Find(soundArray, sound => sound.name == name);
                continue;
            }
            break;
        }


        if (currentMusicTrack != null)
        {
            currentMusicTrack.source.outputAudioMixerGroup = mixerGroups[(int)mixer];
            currentMusicTrack.source.Play();
        }
    }

    public void Play(string name, MixerGroup mixer = MixerGroup.SFX)
    {
        currentSound = null;

        foreach (Sound[] soundArray in soundArrays)
        {
            if (currentSound == null)
            {
                currentSound = Array.Find(soundArray, sound => sound.name == name);
                continue;
            }

            break;
        }

        if (currentSound != null && !currentSound.source.isPlaying)
        {
            currentSound.source.outputAudioMixerGroup = mixerGroups[(int)mixer]; 
            currentSound.source.Play();
        }

        #region More Redundant Code
        /*if (currentSound == null)
        {
            currentSound = Array.Find(music, sound => sound.name == name);

            if (currentSound == null)
            {
                currentSound = Array.Find(bossBarks, sound => sound.name == name);

                if (currentSound == null)
                {
                    currentSound = Array.Find(bossBarks, sound => sound.name == name);

                    if (currentSound == null)
                    {
                        currentSound = Array.Find(peasBarks, sound => sound.name == name);

                        if (currentSound == null)
                        {
                            currentSound = Array.Find(noriBarks, sound => sound.name == name);

                            if (currentSound == null)
                            {
                                currentSound = Array.Find(salmonBarks, sound => sound.name == name);

                                if (currentSound == null)
                                {
                                    currentSound = Array.Find(riceBarks, sound => sound.name == name);

                                    if (currentSound == null)
                                    {
                                        currentSound = Array.Find(restaurantAmb, sound => sound.name == name);

                                        if (currentSound == null)
                                        {
                                            Debug.LogWarning("Sound: '" + name + "' not found! Possibly a typo!");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }*/
        #endregion
    }

    public Sound GetSound(string name)
    {
        Sound queriedSound = null;
        foreach (Sound[] soundArray in soundArrays)
        {
            if (queriedSound == null)
            {
                queriedSound = Array.Find(soundArray, sound => sound.name == name);
                continue;
            }
            break;
        }
        return queriedSound;
    }

    public void PauseSound(string name)
    {
        currentSound.source.Pause();
    }
    public void StopSound(string name)
    {
        Sound activeSound = null;
        foreach (Sound[] soundArray in soundArrays)
        {
            if (activeSound == null)
            {
                activeSound = Array.Find(soundArray, sound => sound.name == name);
                continue;
            }
            break;
        }
        activeSound.source.Stop();
    }
    public void ResumeSound(string name)
    {
        currentSound.source.UnPause();
    }

    //REFACTOR THIS
    public void PlayNormalBossBark()
    {
        int randomBark = Random.Range(9, 16);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayDarkBossBark()
    {
        int randomBark = Random.Range(1, 8);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayNormalNoriBark()
    {
        int randomBark = Random.Range(9, 15);
        Sound currentSound = noriBarks[randomBark];
    }
    public void PlayDarkNoriBark()
    {
        int randomBark = Random.Range(1, 8);
        Sound currentSound = noriBarks[randomBark];
    }
    public void PlayNormalSalmonBark()
    {
        int randomBark = Random.Range(9, 16);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayDarkSalmonBark()
    {
        int randomBark = Random.Range(1, 8);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayNormalRiceBark()
    {
        int randomBark = Random.Range(9, 16);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayDarkRiceBark()
    {
        int randomBark = Random.Range(1, 8);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayNormalPeasBark()
    {
        int randomBark = Random.Range(8, 14);
        Sound currentSound = bossBarks[randomBark];
    }
    public void PlayDarkPeasBark()
    {
        int randomBark = Random.Range(1, 7);
        Sound currentSound = bossBarks[randomBark];
    }
}

