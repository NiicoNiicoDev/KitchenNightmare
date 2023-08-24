using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Wasabi_ExplosiveState : SCR_AI_WasabiBaseStates
{
    float timer;
    SCR_AI_WasabiPea wasabiPeaScript;
    GameObject player;
    GameObject explosionParticles;
    Transform playerTransform;
    NavMeshAgent localMeshAgent;

    bool bCanDealDamage;
    bool bHasDealtDamage;
    bool bHasExploded;

    public override void StartState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Debug.Log("Entered explosive state");

        wasabiPeaScript = wasabiPea.GetComponent<SCR_AI_WasabiPea>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        localMeshAgent = meshAgent;
        explosionParticles = wasabiPeaScript.publicExplosionParticles;

        bCanDealDamage = false;
        bHasDealtDamage = false;
        bHasExploded = false;

        timer = wasabiPeaScript.publicExplosiveTimer;
    }

    public override void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Pea should track the player, then once the timer hits 0, explode and deal damage
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //Explode & set health to 0
            
            bCanDealDamage = Physics.CheckSphere(wasabiPea.transform.position, 2f, wasabiPeaScript.EnemyStats.PlayerLayerMask);
            if (bCanDealDamage && !bHasDealtDamage) //Checks to see if the player was within damage range of the enemy and if the enemy has dealt damage before, this is to stop the damage from the enemy from stacking while switching states
            {
                bHasDealtDamage = true;
                wasabiPeaScript.EnemyStats.PlayerStats.TakeDamage((int)wasabiPeaScript.publicExplosiveDamage);
            }
            wasabiPeaScript.EnemyStats.TakeDamage(wasabiPeaScript.EnemyStats.CurrentHealth);
            explosionParticles = MonoBehaviour.Instantiate(explosionParticles, wasabiPea.transform.position, wasabiPea.transform.rotation);
            explosionParticles.GetComponent<ParticleSystem>().Play();
            MonoBehaviour.Destroy(explosionParticles, 1f);
            bHasExploded = true;
        }
        if(!bHasExploded)
        {
            wasabiPeaScript.AnimationController.SetAnimationBool("MovementState", true);
            wasabiPeaScript.AnimationController.SetAnimationBool("IdleState", false);
            wasabiPeaScript.AnimationController.SetAnimationBool("AttackState", false);
            wasabiPea.transform.LookAt(playerTransform.position);
            localMeshAgent.SetDestination(playerTransform.position);
        }
    }

    public override void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        
    }
}
