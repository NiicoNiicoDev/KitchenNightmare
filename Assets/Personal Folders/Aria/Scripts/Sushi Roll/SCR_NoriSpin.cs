using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responisble for readying the nori sheet object (the object that will collide with the player)
//Movement and spinning is handled in the SCR_AI_Sushi_NoriSpinState.cs script
public class SCR_NoriSpin : MonoBehaviour
{
    Renderer materialRenderer;
    Collider trigger;

    SCR_PlayerStats playerStats;
    float spinDuration;
    int spinDamage;
    bool bIsReady = false;
    bool bHasDealtDamage = false;
    float damageWindow = 1f;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerStats>();
        materialRenderer = GetComponentInChildren<Renderer>();
        materialRenderer.enabled = false;
        trigger = GetComponent<Collider>();
        trigger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(bHasDealtDamage)
        {
            damageWindow -= Time.deltaTime;
            if(damageWindow <= 0f )
            {
                damageWindow = 1f;
                bHasDealtDamage = false;
            }
        }
    }

    public void ReadySpin(int damage, float duration)
    {
        materialRenderer.enabled = true;
        trigger.enabled = true;
        spinDuration = duration;
        spinDamage = damage;
        bIsReady = true;
    }

    public void EndSpin()
    {
        materialRenderer.enabled = false;
        trigger.enabled = false;
        bIsReady= false;
        bHasDealtDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(playerStats == null)
            {
                Debug.LogWarning("Tried to deal damage but player did not have SCR_PlayerStats");
            }
            if(playerStats != null && !bHasDealtDamage)
            {
                playerStats.TakeDamage(spinDamage);
                bHasDealtDamage = true;
            }
        }
    }
}
