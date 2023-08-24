using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Nori_DeathState : SCR_AI_NoriBaseState
{
    SCR_AI_NoriSheet noriSheetScript;
    Collider[] wasabiPeas;
    SCR_AI_WasabiPea wasabiPeaScript;
    SCR_EnemyCounter enemyCounter;
    LayerMask enemyLayerMask;
    GameObject deathParticles;
    GameObject gameManager;
    Renderer renderer;
    bool bHasStartedDeath;
    bool bContainsSalmon;

    public override void StartState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //On death the Sheet should let out a particle effect, then check for Wasabi peas within a set radius and tell them to detonate
        //When all code is finished, the Nori Sheet should be destroyed
        Debug.Log("Nori Sheet Death State");
        if(noriSheetScript == null)
        {
            noriSheetScript = noriSheet.GetComponent<SCR_AI_NoriSheet>();
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
            enemyLayerMask = noriSheetScript.EnemyStats.EnemyLayerMask;
            deathParticles = noriSheetScript.EnemyStats.EnemyDeathParticles;
            renderer = noriSheet.GetComponentInChildren<Renderer>();
            bContainsSalmon = false;
        }
        
        wasabiPeas = Physics.OverlapSphere(noriSheet.transform.position, 15f, enemyLayerMask);
        foreach (var wasabiPea in wasabiPeas)
        {
            if (wasabiPea.transform.name.Contains("AI_SalmonChunk") && wasabiPea.transform.gameObject.activeSelf == true)
            {
                bContainsSalmon = true;
                Debug.Log("Found Salmon Chunk");
            }
        }

        if (!bContainsSalmon)
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
        enemyCounter.numberNoriEnemies--;
        renderer.material.color = Color.red;
        bHasStartedDeath = false;

        noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", false);
        noriSheetScript.StartNoriCoroutine(Death(noriSheet));
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        if(!bHasStartedDeath)
        {
            noriSheetScript.AnimationController.SetAnimationBool("DeathState", true);
            bHasStartedDeath = true;
        }
    }

    public override void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        
    }

    IEnumerator Death(GameObject noriSheet)
    {
        float clipLength = noriSheetScript.DeathAnimation.length;
        yield return new WaitForSeconds(clipLength);
        if (noriSheetScript.EnemyStats.DeathFade != null)
        {
            noriSheetScript.EnemyStats.DeathFade.StartShrinkOut(noriSheet, SCR_ScoreTracker.EnemyType.Nori);
        }
        else
        {
            MonoBehaviour.Destroy(noriSheet); //Must use MonoBehaviour like this as the script does not derive from MonoBehaviour
        }
    }
}
