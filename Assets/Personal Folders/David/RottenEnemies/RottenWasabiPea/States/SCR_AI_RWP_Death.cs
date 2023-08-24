using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RWP_Death : SCR_AI_RottenWasabiPea_BaseStates
{
    private SCR_AI_RottenWasabiPea rottenWasabiScript;

    private GameObject deathParticles;

    private Transform wasabiPeaPosition;

    private Animator animator;

    public override void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        rottenWasabiScript = rottenWasabiPea.GetComponent<SCR_AI_RottenWasabiPea>();

        navMeshAgent.isStopped = true;

        deathParticles = rottenWasabiScript.deathParticles;

        animator = rottenWasabiScript.wasabiAnimator;

        wasabiPeaPosition = rottenWasabiPea.transform;

        rottenWasabiScript.StartRWPCoroutine(DeathSequence(rottenWasabiPea));
    }

    public override void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        
    }

    IEnumerator DeathSequence(GameObject rottenWasabiPea)
    {
        animator.SetTrigger("Dead");

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        float clipLength = clipInfo[0].clip.length;

        Debug.Log("Pea dead");

        MonoBehaviour.Destroy(rottenWasabiPea, clipLength);

        yield return new WaitForSeconds(clipLength - 0.5f); //can't use .IsDestroyed() in update as UpdateState() won't be called again once pea is removed

        GameObject spawnParticles = MonoBehaviour.Instantiate(deathParticles, wasabiPeaPosition.position, wasabiPeaPosition.rotation);

        spawnParticles.GetComponent<ParticleSystem>().Play();

        MonoBehaviour.Destroy(spawnParticles, 1f);
    }
}
