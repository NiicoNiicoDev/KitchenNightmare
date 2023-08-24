using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Death : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    private GameObject deathParticles;

    private Animator animator;

    private Transform noriSheetPosition;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        navMeshAgent.isStopped = true;

        deathParticles = noriSheetScript.deathParticles;

        animator = noriSheetScript.noriAnimator;

        noriSheetPosition = noriSheet.transform;

        noriSheetScript.StartRNSCoroutine(DeathSequence(noriSheet));
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        
    }

    IEnumerator DeathSequence(GameObject rottenNori)
    {
        animator.SetTrigger("Dead");

        float clipLength = noriSheetScript.deathAnimation.length;

        yield return new WaitForSeconds(clipLength); //can't use .IsDestroyed() in update as UpdateState() won't be called again once pea is removed

        if (noriSheetScript.healthScript.DeathFade != null)
        {
            noriSheetScript.healthScript.DeathFade.StartShrinkOut(rottenNori, SCR_ScoreTracker.EnemyType.RottenNori);
        }
        else
        {
            MonoBehaviour.Destroy(rottenNori);
        }

        /*GameObject spawnParticles = MonoBehaviour.Instantiate(deathParticles, noriSheetPosition.position, noriSheetPosition.rotation);

        spawnParticles.GetComponent<ParticleSystem>().Play();

        *//*Color fade = rottenNori.GetComponentInChildren<Renderer>().material.color;

        for (float i = 1f; i >= 0f; i -= 0.02f)
        {
            fade.a = i;
            rottenNori.GetComponentInChildren<Renderer>().material.color = fade;
            yield return new WaitForSeconds(0.1f);
        }*//*

        MonoBehaviour.Destroy(spawnParticles, 1f);

        MonoBehaviour.Destroy(rottenNori);*/
    }
}
