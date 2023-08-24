using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_SalmonStompState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    SCR_PlayerStats playerStats;
    Rigidbody rb;
    Transform sushiTransform;
    float startingHeight;
    LayerMask playerLayerMask;
    GameObject aoeObject;

    int damage;
    float recoveryTime;
    float jumpHeight;
    float jumpForce;
    float delay;

    bool bHasSlammedDown;
    bool bHasReachedPeak;
    bool bHasDealtDamage;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Salmon Stomp State");
        if (sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            rb = sushiRoll.GetComponent<Rigidbody>();
            sushiTransform = sushiRoll.transform;
            jumpForce = sushiRollScript.StompJumpForce;
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerStats>();
            playerLayerMask = sushiRollScript.PlayerLayerMask;
        }

        recoveryTime = sushiRollScript.RecoveryTimer;
        damage = sushiRollScript.SalmonStompDamage;
        meshAgent.isStopped = true;
        meshAgent.enabled = false;
        rb.useGravity = false;
        delay = 1f;
        startingHeight = sushiTransform.localPosition.y;
        jumpHeight = startingHeight + sushiRollScript.StompJumpHeight;

        aoeObject = MonoBehaviour.Instantiate(sushiRollScript.EnemyStats.AOEObject, sushiTransform.position, sushiTransform.localRotation);
        aoeObject.transform.localScale = new Vector3(1.5f, 1f, 1.5f);

        bHasReachedPeak = false;
        bHasSlammedDown = false;
        bHasDealtDamage = false;

        sushiRollScript.AudioManager.PlaySound(sushiRollScript.AudioManager.AudioClips[6], false);
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if (sushiRollScript.GameManager.bPlayerDead)
        {
            rb.useGravity = true;
            sushiTransform.localPosition = new Vector3(sushiTransform.localPosition.x, startingHeight, sushiTransform.localPosition.z);
            meshAgent.enabled = true;
            MonoBehaviour.Destroy(aoeObject);
            return;
        }

        //Jump up in the air
        if (!bHasReachedPeak)
        {
            sushiTransform.localPosition += Vector3.up * jumpForce * Time.deltaTime;
            if(sushiTransform.localPosition.y >= jumpHeight)
            {
                bHasReachedPeak = true;
            }
        }
        //Wait a second
        if(bHasReachedPeak && !bHasSlammedDown)
        {
            delay -= Time.deltaTime;
            if(delay <= 0f)
            {
                //Start moving down
                sushiTransform.localPosition -= Vector3.up * (jumpForce * 2) * Time.deltaTime;
                if (sushiTransform.localPosition.y <= startingHeight + 0.5f)
                {
                    rb.useGravity = true;
                    sushiTransform.localPosition = new Vector3(sushiTransform.localPosition.x, startingHeight, sushiTransform.localPosition.z);
                    //Check for player and deal damage
                    if(playerStats != null)
                    {
                        if (Physics.CheckSphere(sushiTransform.localPosition, 0.5f, playerLayerMask) && !bHasDealtDamage)
                        {
                            //Deal damage to the player
                            playerStats.TakeDamage(damage + sushiRollScript.EnemyStats.EnemyDamageMod);
                        }

                        if (Physics.CheckSphere(sushiTransform.localPosition, 1.5f, playerLayerMask))
                        {
                            //Stun the player (this has not been implemented yet
                            playerStats.StunPlayer(1f, false);
                        }
                    }
                    
                    bHasSlammedDown = true;
                }
            }
        }

        //slam down and start recovery
        if(bHasSlammedDown)
        {
            recoveryTime -= Time.deltaTime;
            if(aoeObject) MonoBehaviour.Destroy(aoeObject);
            if (recoveryTime <= 0)
            {
                meshAgent.enabled = true;
                sushiRollScript.currentState = sushiRollScript.movementState;
                sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }
}
