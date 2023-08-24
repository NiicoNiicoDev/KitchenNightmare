using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//TODO: Decide a suitable animation while in stick state
public class SCR_AI_RRG_Sticking : SCR_AI_RRG_BaseStates
{
    private SCR_AI_RottenRiceGrain riceGrainScript;

    private SCR_PlayerStats playerHealthScript;

    private GameObject explosionParticles;

    private float timer = 0f;

    private bool readyToExplode = false;

    private Rigidbody rb;

    private bool bUnstuck = false;

    public override void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        bUnstuck = false;

        riceGrainScript = rottenRiceGrain.GetComponent<SCR_AI_RottenRiceGrain>();

        riceGrainScript.riceGrainAnimator.SetBool("IsMoving", false);

        riceGrainScript.unstick = false;

        playerHealthScript = riceGrainScript.playerHealthScript;

        rb = rottenRiceGrain.GetComponent<Rigidbody>();

        timer = 0f;

        readyToExplode = false;

        playerHealthScript.DamageOverTime(riceGrainScript.attackDamage, riceGrainScript.stickTime, riceGrainScript.damageDelay);

        playerHealthScript.SlowPlayer(riceGrainScript.slowdownPercentage, riceGrainScript.stickTime);
    }

    public override void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        timer += Time.deltaTime;

        rottenRiceGrain.transform.position = riceGrainScript.player.transform.position + riceGrainScript.stickOffset;

        //rottenRiceGrain.transform.LookAt(riceGrainScript.player.transform);

        if(timer > riceGrainScript.stickTime && !readyToExplode)
        {
            readyToExplode = true;

            MonoBehaviour.Destroy(rottenRiceGrain, 0.5f);

            explosionParticles = MonoBehaviour.Instantiate(riceGrainScript.explosionParticles, rottenRiceGrain.transform.position, rottenRiceGrain.transform.rotation);

            explosionParticles.GetComponent<ParticleSystem>().Play();

            MonoBehaviour.Destroy(explosionParticles, 1f);

            playerHealthScript.TakeDamage(riceGrainScript.explosionDamage);
        } else if (riceGrainScript.unstick && !readyToExplode && !bUnstuck)
        {
            //Debug.Log("UNSTICK RICE GRAIN");
            bUnstuck = true;
            playerHealthScript.ResetSpeed();
            Vector3 direction = rottenRiceGrain.transform.position - riceGrainScript.player.transform.position;
            direction.Normalize();
            //navMeshAgent.isStopped = true;
            rb.AddForce(direction * 2f, ForceMode.Impulse);
            riceGrainScript.riceGrainAnimator.SetTrigger("Damaged");
            riceGrainScript.EnterState(riceGrainScript.idle);
            riceGrainScript.timeSinceLastAttack = 0f;
        }
    }
}
