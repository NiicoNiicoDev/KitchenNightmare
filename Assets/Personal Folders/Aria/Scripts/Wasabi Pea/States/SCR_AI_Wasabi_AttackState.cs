using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Wasabi_AttackState : SCR_AI_WasabiBaseStates
{
    SCR_AI_WasabiPea wasabiPeaScript;
    int attackDamage;
    float timerDelay;
    float timerAttack;

    GameObject player;
    Rigidbody rigidbody;

    bool bCanDealDamage = false;
    bool bHasDealtDamage = false;
    bool bHasAppliedForce = false;
    

    public override void StartState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Debug.Log("Wasabi Attack State Start");
        if(wasabiPeaScript == null) //Similarly to the movement state, this ensures that values are only assigned once when the start function is first called
        {
            wasabiPeaScript = wasabiPea.GetComponent<SCR_AI_WasabiPea>();
            rigidbody = wasabiPea.GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player");
            attackDamage = (int)wasabiPeaScript.publicAttackDamage;
        }

        //These values are set every time the start fucntion is called, this is essential for the boolean and the timer values
        timerDelay = wasabiPeaScript.publicAttackChargeTimer;
        timerAttack = wasabiPeaScript.publicAttackHangTimer;
        meshAgent.isStopped = true;
        bCanDealDamage = false;
        bHasDealtDamage = false;
        bHasAppliedForce = false;

        wasabiPeaScript.AnimationController.SetAnimationBool("MovementState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("IdleState", false);
        wasabiPeaScript.AnimationController.SetAnimationBool("AttackState", true);

        wasabiPeaScript.AudioManager.PlayRandomSound();
    }

    public override void UpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        //Debug.Log("Wasabi Attack State Update");

        if(wasabiPeaScript.EnemyStats.IsStunned)
        {
            //Cancel the attack and switch back to the movement state
            bCanDealDamage = false;
            wasabiPeaScript.currentState = wasabiPeaScript.movementState;
            wasabiPeaScript.currentState.StartState(wasabiPea.gameObject, meshAgent);
        }

        if (!bHasAppliedForce) //Checks to see if force has been applied to the enemy
        {
            timerDelay -= Time.deltaTime;
            if(timerDelay <= 0f)
            {
                //Debug.Log("Applying forward force");
                rigidbody.AddForce(wasabiPea.transform.forward * 5, ForceMode.Impulse); //Applies an impulse force to the enemy in the forward direction reletive to the player
                bHasAppliedForce = true; //Sets bHasAppliedForce to true so that the force 
                
            }
        }
        
        if(bHasAppliedForce)
        {
            
            timerAttack -= Time.deltaTime;

            
            bCanDealDamage = Physics.CheckSphere(wasabiPea.transform.position, 2f, wasabiPeaScript.EnemyStats.PlayerLayerMask);
            if (bCanDealDamage && !bHasDealtDamage) //Checks to see if the player was within damage range of the enemy and if the enemy has dealt damage before, this is to stop the damage from the enemy from stacking while switching states
            {
                bHasDealtDamage = true;
                wasabiPeaScript.EnemyStats.PlayerStats.TakeDamage(attackDamage + wasabiPeaScript.EnemyStats.EnemyDamageMod);
                
                /*
                wasabiPeaScript.currentState = wasabiPeaScript.movementState;
                wasabiPeaScript.currentState.StartState(wasabiPea.gameObject, meshAgent);
                */
            }
            

            if(timerAttack <= 0f) //Once the timer has reached 0, switch states
            {
                wasabiPeaScript.currentState = wasabiPeaScript.movementState;
                wasabiPeaScript.currentState.StartState(wasabiPea.gameObject, meshAgent);
            }
        }
        

        
    }

    public override void FixedUpdateState(GameObject wasabiPea, NavMeshAgent meshAgent)
    {
        
    }
}
