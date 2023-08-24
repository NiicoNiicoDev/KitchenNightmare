using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RWP_Damaged : SCR_AI_RottenWasabiPea_BaseStates
{
    private SCR_AI_RottenWasabiPea rottenWasabiScript;

    private float timer = 0f;

    private Rigidbody wasabiRB;

    public override void StartState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        //Debug.Log("Damaged State");
        rottenWasabiScript = rottenWasabiPea.GetComponent<SCR_AI_RottenWasabiPea>();
        
        timer = 0f;

        rottenWasabiScript.wasabiAnimator.SetTrigger("Damaged");

        wasabiRB = rottenWasabiPea.GetComponent<Rigidbody>();

        wasabiRB.AddForce(rottenWasabiPea.transform.forward * rottenWasabiScript.attackPower, ForceMode.Impulse);
        
        if(rottenWasabiScript.chanceOfMultiply <= Random.Range(1, 101) && rottenWasabiScript.canMultiply)
        {
            MultiplyWasabiPea(rottenWasabiPea);
        }

        rottenWasabiScript.canMultiply = false;
    }

    public override void UpdateState(GameObject rottenWasabiPea, NavMeshAgent navMeshAgent)
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            rottenWasabiScript.healthScript.justDamaged = false;
        }
    }

    void MultiplyWasabiPea(GameObject rottenWasabiPea)
    {
        for(int i = 0; i < Random.Range(1, 4); i++)
        {
            Vector3 spawnPoint = rottenWasabiPea.transform.position + new Vector3((i + 1f) * 2, 0f, 0f);
            GameObject spawnedPea = (GameObject)MonoBehaviour.Instantiate(rottenWasabiPea, spawnPoint, rottenWasabiPea.transform.rotation);
            spawnedPea.GetComponent<SCR_AI_RottenWasabiPea>().canMultiply = false;
        }
    }
}
