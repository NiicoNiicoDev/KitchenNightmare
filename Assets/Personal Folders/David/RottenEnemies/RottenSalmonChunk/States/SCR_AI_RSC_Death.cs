using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_Death : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    private GameObject deathParticles;

    private Animator animator;

    private Transform salmonPosition;

    private bool bDeathSequenceStarted = false;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        navMeshAgent.isStopped = true;

        deathParticles = salmonChunkScript.deathParticles;

        animator = salmonChunkScript.salmonAnimator;

        salmonPosition = salmonChunk.transform;
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        if (!bDeathSequenceStarted)
        {
            salmonChunkScript.StartRSCCoroutine(DeathSequence(salmonChunk));
        }
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        
    }

    IEnumerator DeathSequence(GameObject salmonChunk)
    {
        bDeathSequenceStarted = true;

        animator.SetTrigger("Dead");

        float clipLength = salmonChunkScript.deathAnimation.length;

        yield return new WaitForSeconds(clipLength); //can't use .IsDestroyed() in update as UpdateState() won't be called again once pea is removed

        if (salmonChunkScript.healthScript.DeathFade != null)
        {
            salmonChunkScript.healthScript.DeathFade.StartShrinkOut(salmonChunk, SCR_ScoreTracker.EnemyType.RottenSalmon);
        }
        else
        {
            MonoBehaviour.Destroy(salmonChunk);
        }

        /*GameObject spawnParticles = MonoBehaviour.Instantiate(deathParticles, salmonPosition.position, salmonPosition.rotation);

        spawnParticles.GetComponent<ParticleSystem>().Play();

        *//*Color fade = salmonChunk.GetComponent<Renderer>().material.color;

        for (float i = 1f; i >= 0f; i -= 0.02f)
        {
            fade.a = i;
            salmonChunk.GetComponent<Renderer>().material.color = fade;
            yield return null;
        }*//*

        MonoBehaviour.Destroy(spawnParticles, 1f);

        MonoBehaviour.Destroy(salmonChunk);*/
    }
}
