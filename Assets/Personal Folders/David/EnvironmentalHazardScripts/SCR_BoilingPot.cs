using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the behaviour of the boiling pot environmental hazard
public class SCR_BoilingPot : MonoBehaviour
{
    //Damage that the hazard inflicts on player if hit
    [SerializeField] private int damage = 1;

    //How close the player must be when attacking to spill the pot
    [SerializeField] private float activationRange = 1f;

    //Animation which plays when the pot spills
    [SerializeField] private Animator spillAnimation;

    //name used for the animation trigger
    [SerializeField] private string triggerName = "hasSpilt";

    [HideInInspector] public bool bDestroyPot = false;

    //reference to the player's game object
    private GameObject player;

    //reference to the attack script
    private PSM_InputHandler inputScript;

    //reference to the health script
    private SCR_PlayerStats healthScript;

    // Start is called before the first frame update
    void Start()
    {
        //find player in the scene and store it as player
        player = GameObject.FindWithTag("Player");

        inputScript = player.GetComponent<PSM_InputHandler>();

        healthScript = player.GetComponent<SCR_PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            //if the player is attacking and is within the activation range of the pot
            if ((inputScript.abilityOneAction.IsPressed() || inputScript.abilityTwoAction.IsPressed() || inputScript.abilityThreeAction.IsPressed()) && Vector3.Distance(transform.position, player.transform.position) < activationRange)
            {
                //attackScript.bAttacking = false;

                //spill the pot
                Spill();
            }
        }

        if (bDestroyPot)
        {
            Destroy(gameObject);
        }
    }

    //controls the behaviour of the pot when it is spilt
    void Spill()
    {
        Debug.Log("SPILL");

        //if there is a spill animation
        if (spillAnimation)
        {
            spillAnimation.SetTrigger(triggerName);

            AnimatorClipInfo[] clipInfo = spillAnimation.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                float clipLength = clipInfo[0].clip.length;
                //then destroy the pot once the animation is finished
                Destroy(gameObject, clipLength);
            }
            else
            {
                Destroy(gameObject);
            }
        } else
        {
            //otherwise, just destroy the pot as the animation is not available
            Destroy(gameObject);
        }

        //call the damage function in the health script
        //TODO: call the health script used in the final game
        healthScript.TakeDamage(damage);
    }
}
