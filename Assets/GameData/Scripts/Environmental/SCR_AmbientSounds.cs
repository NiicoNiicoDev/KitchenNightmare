using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AmbientSounds : MonoBehaviour
{
    [SerializeField] private float distanceToPlayer = 10.0f;

    private Transform _transform;
    private Transform player;

    private bool bSoundPlaying = false;
    private string soundName;

    private void Start()
    {
        _transform = transform;
        player = FindObjectOfType<SCR_PlayerStats>().transform;

        int rand = Random.Range(0, SCR_AudioManager.instance.restaurantAmb.Length);
        soundName = SCR_AudioManager.instance.restaurantAmb[rand].name;
    }

    private void Update()
    {
        if (Vector3.Distance(_transform.position, player.position) <= distanceToPlayer)
        {
            if (!bSoundPlaying)
            {
                SCR_AudioManager.instance.Play(soundName);
                bSoundPlaying = true;
            }
        }
        else if (bSoundPlaying)
        {
            SCR_AudioManager.instance.StopSound(soundName);
            bSoundPlaying = false;
        }
    }
}
