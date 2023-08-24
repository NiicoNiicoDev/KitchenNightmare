using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RRG_Death : SCR_AI_RRG_BaseStates
{
    private SCR_AI_RottenRiceGrain riceGrainScript;

    private Animator animator;

    public override void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        riceGrainScript = rottenRiceGrain.GetComponent<SCR_AI_RottenRiceGrain>();

        animator = riceGrainScript.riceGrainAnimator;

        SCR_PlayerStats playerHealthScript = riceGrainScript.playerHealthScript;

        playerHealthScript.ResetSpeed();

        riceGrainScript.StartRRGCoroutine(DeathSequence(rottenRiceGrain));
    }

    public override void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        
    }

    IEnumerator DeathSequence(GameObject rottenRiceGrain)
    {
        animator.SetTrigger("Dead");

        float clipLength = riceGrainScript.deathAnimation.length;

        yield return new WaitForSeconds(clipLength);

        if (riceGrainScript.healthScript.DeathFade != null)
        {
            riceGrainScript.healthScript.DeathFade.StartShrinkOut(rottenRiceGrain, SCR_ScoreTracker.EnemyType.RottenRice);
        }
        else
        {
            MonoBehaviour.Destroy(rottenRiceGrain);
        }

        /*GameObject deathParticles = MonoBehaviour.Instantiate(riceGrainScript.deathParticles, riceGrainScript.deathParticleSpawn.transform.position, riceGrainScript.deathParticleSpawn.transform.rotation);

        deathParticles.GetComponent<ParticleSystem>().Play();

        MonoBehaviour.Destroy(deathParticles, 1f);

        MonoBehaviour.Destroy(rottenRiceGrain);*/
    }


}
