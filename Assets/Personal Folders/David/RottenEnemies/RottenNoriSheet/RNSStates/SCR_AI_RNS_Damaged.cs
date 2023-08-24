using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Damaged : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    private float attackTime;

    private GameObject spawnedNori;

    private float timeToSplit = 0f;

    private Vector3 spawnPoint;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        noriSheetScript.noriAnimator.SetTrigger("Damaged");

        timeToSplit = 0f;

        AnimatorClipInfo[] clipInfo = noriSheetScript.noriAnimator.GetCurrentAnimatorClipInfo(0);
        attackTime = clipInfo[0].clip.length;

        SplitNori(noriSheet);
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        
    }

    public IEnumerator StartFleeing(GameObject noriSheet)
    {
        yield return null;
        spawnedNori.GetComponent<SCR_EnemyStats>().CurrentHealth = 1;
        spawnedNori.GetComponent<SCR_AI_RNS>().EnterState(noriSheetScript.damaged);

        Vector3 endPoint = noriSheet.transform.position + (noriSheet.transform.TransformDirection(Vector3.forward) * 2);

        float t = 0f;

        while(t < 1)
        {
            spawnedNori.transform.position = Vector3.Lerp(spawnPoint, endPoint, t);
            spawnedNori.transform.rotation = noriSheet.transform.rotation;
            t += Time.deltaTime * 3f;
            timeToSplit += Time.deltaTime;
            yield return null;
        }
        //noriSheet.GetComponent<Collider>().isTrigger = false;
        yield return new WaitForSeconds(attackTime - timeToSplit);

        Debug.Log("Start Fleeing");
        noriSheetScript.EnterState(noriSheetScript.flee);
    }

    void SplitNori(GameObject noriSheet)
    {
        if (noriSheetScript.canMultiply)
        {
            //noriSheet.GetComponent<Collider>().isTrigger = true;
            spawnPoint = noriSheet.transform.position + new Vector3(0.4f, 0f, 0f);
            spawnedNori = MonoBehaviour.Instantiate(noriSheet, spawnPoint, noriSheet.transform.rotation);
            //spawnedNori.GetComponent<Collider>().isTrigger = false;
            spawnedNori.GetComponent<SCR_AI_RNS>().canMultiply = false;
            spawnedNori.GetComponent<SCR_AI_RNS>().leader = false;
            spawnedNori.GetComponent<SCR_AI_RNS>().leadNori = noriSheet;
            spawnedNori.transform.localScale *= 0.5f;
            noriSheet.transform.localScale *= 0.9f;
            noriSheetScript.StartRNSCoroutine(StartFleeing(noriSheet));
        }
        else
        {
            noriSheetScript.EnterState(noriSheetScript.flee);
        }
    }
}
