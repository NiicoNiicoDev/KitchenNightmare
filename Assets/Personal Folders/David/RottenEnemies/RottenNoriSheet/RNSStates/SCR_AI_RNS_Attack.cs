using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Attack : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    private SCR_PlayerStats playerHealthScript;

    private float attackTime;

    private RaycastHit hit;

    private bool bDamagedPlayer = false;

    private float timer;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        playerHealthScript = noriSheetScript.player.GetComponent<SCR_PlayerStats>();

        timer = 0f;

        noriSheetScript.noriAnimator.SetTrigger("Attacking");

        AnimatorClipInfo[] clipInfo = noriSheetScript.noriAnimator.GetCurrentAnimatorClipInfo(0);
        attackTime = clipInfo[0].clip.length;

        bDamagedPlayer = false;

        noriSheetScript.StartRNSCoroutine(StopAttacking());
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        timer += Time.deltaTime;
        if(Physics.CheckSphere(noriSheet.transform.position + new Vector3(1f, 0f, 0f), noriSheetScript.attackRadius, noriSheetScript.playerLM) && !bDamagedPlayer && timer > attackTime/2){
            playerHealthScript.TakeDamage(noriSheetScript.attackDamage);

            bDamagedPlayer = true;
        }
    }

    public IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(attackTime);

        noriSheetScript.EnterState(noriSheetScript.flee);
    }
}
