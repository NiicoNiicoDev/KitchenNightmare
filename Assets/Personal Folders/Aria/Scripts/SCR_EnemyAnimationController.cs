using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SCR_EnemyAnimationController : MonoBehaviour
{
    public Animator animator;

    public bool BAnimationReadyForAttack { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationBool(string animName, bool animationBool)
    {
        //Debug.Log("Setting Parameter " + animName + " to " + animationBool);
        animator.SetBool(animName, animationBool);
    }
}
