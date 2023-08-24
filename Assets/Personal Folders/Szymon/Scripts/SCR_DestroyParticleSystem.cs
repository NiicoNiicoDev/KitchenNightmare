using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_DestroyParticleSystem : MonoBehaviour
{
    void Start()
    {
        //Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
    }

}
