using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_DeathState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    Collider[] wasabiPeas;
    SCR_AI_WasabiPea wasabiPeaScript;
    SCR_EnemyCounter enemyCounter;
    LayerMask enemyLayerMask;
    GameObject deathParticles;
    GameObject gameManager;
    Renderer renderer;
    bool bHasStartedDeath;
    bool bContainsNori;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        //Debug.Log("Salmon Chunk Death State");
        if(salmonChunkScript == null)
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>();
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
            enemyLayerMask = salmonChunkScript.EnemyStats.EnemyLayerMask;
            deathParticles = salmonChunkScript.EnemyStats.EnemyDeathParticles;
            renderer = salmonChunk.GetComponentInChildren<Renderer>();
            bContainsNori = false;
        }

        wasabiPeas = Physics.OverlapSphere(salmonChunk.transform.position, 15f, enemyLayerMask);
        foreach (var wasabiPea in wasabiPeas)
        {
            //Debug.Log(wasabiPea.transform.name);
            if(wasabiPea.transform.name.Contains("AI_NoriSheet") && wasabiPea.transform.gameObject.activeSelf == true)
            {
                bContainsNori = true;
                Debug.Log("Found Nori Sheet");
            }
        }

        if(!bContainsNori)
        {
            foreach (var wasabiPea in wasabiPeas)
            {
                if (wasabiPea.transform.name.Contains("AI_WasabiPea") && wasabiPea.transform.gameObject.activeSelf == true)
                {
                    wasabiPeaScript = wasabiPea.gameObject.GetComponent<SCR_AI_WasabiPea>();
                    wasabiPeaScript.bSwitchToExplosiveState = true;
                }
            }
        }

        meshAgent.isStopped = true;
        enemyCounter.numberSalmonEnemies--;
        renderer.material.color = Color.red;
        bHasStartedDeath = false;

        salmonChunkScript.AnimationController.SetAnimationBool("IdleState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SpinAttackState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("StrikeAttackState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SlapAttackState", false);
        salmonChunkScript.StartSalmonCoroutine(Death(salmonChunk));
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        if (!bHasStartedDeath)
        {
            salmonChunkScript.AnimationController.SetAnimationBool("DeathState", true);
            bHasStartedDeath = true;
        }
    }

    public override void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {

    }

    IEnumerator Death(GameObject salmonChunk)
    {
        float clipLength = salmonChunkScript.DeathAnimation.length;
        yield return new WaitForSeconds(clipLength);
        if (salmonChunkScript.EnemyStats.DeathFade != null)
        {
            salmonChunkScript.EnemyStats.DeathFade.StartShrinkOut(salmonChunk, SCR_ScoreTracker.EnemyType.Salmon);
        }
        else
        {
            MonoBehaviour.Destroy(salmonChunk); //Must use MonoBehaviour like this as the script does not derive from MonoBehaviour
        }
    }
}
