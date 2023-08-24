using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Wasabi_DeathState : SCR_AI_WasabiBaseStates
{
    GameObject gameManager;
    //GameObject deathParticles;
    GameObject puddle;
    SCR_EnemyCounter enemyCounter;
    SCR_AI_WasabiPea wasabiPeaScript;
    bool hasStartedDeath;
    Renderer renderer;

    public override void StartState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Debug.Log("Entered Death State");
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        enemyCounter = gameManager.GetComponent<SCR_EnemyCounter>();
        wasabiPeaScript = wasabiPea.GetComponent<SCR_AI_WasabiPea>();
        renderer = wasabiPea.GetComponent<Renderer>();

        //deathParticles = wasabiPeaScript.publicDeathParticles;
        puddle = wasabiPeaScript.puddleObject;
        hasStartedDeath = false;

        wasabiPeaScript.AnimationController.SetAnimationBool("MovementState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("IdleState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("AttackState", false);

        wasabiPeaScript.AudioManager.PlayRandomSound();

        wasabiPeaScript.StartWasabiCoroutine(Death(wasabiPea));
    }

    public override void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Decreases the total number of Wasabi Pea enemies in the enemyCounter script
        if(!hasStartedDeath)
        {
            meshAgent.isStopped = true;
            renderer.material.color = Color.red;
            enemyCounter.numberWasabiEnemies--;
            wasabiPeaScript.AnimationController.SetAnimationBool("DeathState", true);

            hasStartedDeath = true;
        }
    }

    public override void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        
    }

    IEnumerator Death(GameObject wasabiPea)
    {
        float clipLength = wasabiPeaScript.DeathAnimation.length;
        yield return new WaitForSeconds(clipLength);

        if(wasabiPeaScript.EnemyStats.DeathFade != null)
        {
            wasabiPeaScript.EnemyStats.DeathFade.StartShrinkOut(wasabiPea, SCR_ScoreTracker.EnemyType.Wasabi);
            if(puddle)
            {
                puddle = MonoBehaviour.Instantiate(puddle, wasabiPea.transform.localPosition, wasabiPea.transform.localRotation);
                puddle.transform.localPosition = new Vector3(puddle.transform.localPosition.x, 0.1f, puddle.transform.localPosition.z);
                MonoBehaviour.Destroy(puddle, puddle.GetComponentInChildren<ParticleSystem>().main.duration);
            }
            
        }
        else
        {
            MonoBehaviour.Destroy(wasabiPea); //Must use MonoBehaviour like this as the script does not derive from MonoBehaviour
        }
        
       
    }
}
