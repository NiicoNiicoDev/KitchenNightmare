using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_DeathState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    SCR_EnemyStats enemyHealth;
    SCR_DissolveController dissolveController;
    Collider[] wasabiPeas;
    LayerMask enemyLayerMask;
    GameObject deathParticles;
    Renderer renderer;
    ParticleSystem particleSystem;
    bool bHasStartedDeath;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Death State");
        if(sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            renderer = sushiRoll.GetComponentInChildren<Renderer>();
            enemyLayerMask = sushiRollScript.EnemyLayerMask;
            deathParticles = sushiRollScript.DeathParticles;
            dissolveController = sushiRoll.GetComponent<SCR_DissolveController>();
        }

        wasabiPeas = Physics.OverlapSphere(sushiRoll.transform.position, 50f, enemyLayerMask);
        foreach (var wasabiPea in wasabiPeas) //Kills off all the wasabi peas in the nearby area as the battle is over and they are no longer needed
        {
            if (wasabiPea.transform.name.Contains("AI_WasabiPea") && wasabiPea.transform.gameObject.activeSelf == true)
            {
                enemyHealth = wasabiPea.gameObject.GetComponent<SCR_EnemyStats>();
                enemyHealth.TakeDamage(enemyHealth.CurrentHealth);
            }
        }

        meshAgent.isStopped = true;
        renderer.material.color = Color.red;
        bHasStartedDeath = false;

        sushiRollScript.AnimationController.SetAnimationBool("IdleState", false);
        sushiRollScript.AnimationController.SetAnimationBool("MovementState", false);
        GameManager.gameManager.PlayBossDefeatedMusic();
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if(!bHasStartedDeath)
        {
            /*deathParticles = MonoBehaviour.Instantiate(deathParticles, sushiRoll.transform.position, sushiRoll.transform.rotation);
            deathParticles.transform.localScale = Vector3.one * 5f;
            particleSystem = deathParticles.GetComponent<ParticleSystem>();
            particleSystem.loop = true;
            particleSystem.Play();*/
            sushiRollScript.AnimationController.SetAnimationBool("DeathState", true);
            /*if(sushiRollScript.EnemyStats.DeathFade != null)
            {
                sushiRollScript.EnemyStats.DeathFade.StartShrinkOut(sushiRoll, SCR_ScoreTracker.EnemyType.Boss);
            }*/

            //MonoBehaviour.Destroy(deathParticles, 2f);
            bHasStartedDeath = true;
        }

        if(sushiRollScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && bHasStartedDeath)
        {
            /*if(sushiRollScript.EnemyStats.DeathFade == null)
            {
                GameManager.gameManager.LevelEnded();
                MonoBehaviour.Destroy(sushiRoll, 1f); //Only destroy the sushi roll after the animation has finished playing
            }*/

            if (dissolveController != null)
            {
                dissolveController.StartCoroutine(dissolveController.Dissolve());
            }
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }
}
