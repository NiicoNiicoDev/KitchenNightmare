using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RWP_Attack : SCR_AI_RottenWasabiPea_BaseStates
{
    private Rigidbody wasabiRB;

    private float timer = 0f;

    private bool justAttacked = false;

    private SCR_AI_RottenWasabiPea rottenWasabiScript;

    public override void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Attack State");
        wasabiRB = rottenWasabiPea.GetComponent<Rigidbody>();
        rottenWasabiScript = rottenWasabiPea.GetComponent<SCR_AI_RottenWasabiPea>();
        navMeshAgent.isStopped = true;
        timer = 0f;
        justAttacked = false;

        rottenWasabiScript.wasabiAnimator.SetTrigger("Attacking");
    }

    public override void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        if(!justAttacked && timer > 0.5f)
        {
            justAttacked = true;

            navMeshAgent.isStopped = false;

            wasabiRB.AddForce(rottenWasabiPea.transform.forward * rottenWasabiScript.attackPower, ForceMode.Impulse);

            timer = 0f;

            return;
        }

        timer += Time.deltaTime;

        if(timer > rottenWasabiScript.attackFrequency)
        {
            justAttacked = false;
        }
    }
}
