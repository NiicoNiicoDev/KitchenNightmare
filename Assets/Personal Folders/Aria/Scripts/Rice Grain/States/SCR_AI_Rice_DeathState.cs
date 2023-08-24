using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Rice_DeathState : SCR_AI_RiceBaseStates
{
    GameObject gameManager;
    GameObject deathParticles;
    SCR_EnemyCounter enemyCounter;
    SCR_AI_RiceGrain riceGrainScript;
    bool bHasStartedDeath;
    Renderer renderer;

    public override void StartState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        //Debug.Log("Entered Rice Death State");
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
        riceGrainScript = riceGrain.GetComponent<SCR_AI_RiceGrain>();
        renderer = riceGrain.GetComponentInChildren<Renderer>();

        //deathParticles = riceGrainScript.deathParticles;

        bHasStartedDeath = false;

        riceGrainScript.AnimationController.SetAnimationBool("IdleState", false);
        riceGrainScript.AnimationController.SetAnimationBool("MovementState", false);
        riceGrainScript.AnimationController.SetAnimationBool("BiteAttackState", false);
        riceGrainScript.AnimationController.SetAnimationBool("ChargeAttackState", false);
        riceGrainScript.StartRiceCoroutine(Death(riceGrain));
    }

    public override void UpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        if (!bHasStartedDeath)
        {
            meshAgent.isStopped = true;
            renderer.material.color = Color.red;
            enemyCounter.numberRiceEnemies--;
            
            riceGrainScript.AnimationController.SetAnimationBool("DeathState", true);
            bHasStartedDeath = true;
        }
    }

    public override void FixedUpdateState(GameObject riceGrain, NavMeshAgent meshAgent)
    {
        
    }

    IEnumerator Death(GameObject riceGrain)
    {
        float clipLength = riceGrainScript.DeathAnimation.length;
        yield return new WaitForSeconds(clipLength);
        if (riceGrainScript.EnemyStats.DeathFade != null)
        {
            riceGrainScript.EnemyStats.DeathFade.StartShrinkOut(riceGrain, SCR_ScoreTracker.EnemyType.Rice);
        }
        else
        {
            MonoBehaviour.Destroy(riceGrain); //Must use MonoBehaviour like this as the script does not derive from MonoBehaviour
        }
    }
}
