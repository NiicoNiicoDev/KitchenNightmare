using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is depreciated
//This script will be attached to each enemy
//When an enemy needs to be stunned this script can be accessed by the player
//Each individual enemy can watch their own Stun script and handle the logic for being stunned internally
public class SCR_StunEnemy : MonoBehaviour
{
    [Tooltip("Use the hotkey to test stunning on this enemy, hotkey is F5")]
    [SerializeField] bool bUseTestingHotkey = false;
    [Tooltip("Duration of the test stun")]
    [SerializeField] float testStunDurtaion = 1f;
    
    [Tooltip("The time after being stunned until the enemy can be stunned again, this prevents stun-locking")]
    [SerializeField] float stunInvulWindow = 1f;

    public bool IsStunned { get; private set; }

    float stunDuration;
    float windowTimer;
    bool bCanBeStunned = true;

    // Start is called before the first frame update
    void Start()
    {
        windowTimer = stunInvulWindow;
    }

    // Update is called once per frame
    void Update()
    {
        //If the enemy is stunned, decrease the timer until 0 then end the stun
        if(IsStunned)
        {
            stunDuration -= Time.deltaTime;
            if(stunDuration <= 0f)
            {
                //Enemy is no longer stunned
                IsStunned = false;
                Debug.Log("Stun Ended");
            }
        }

        if(!IsStunned && !bCanBeStunned)
        {
            //The enemy is no longer stunned but cannot be stunned again until the timer reaches 0
            windowTimer -= Time.deltaTime;
            if(windowTimer <= 0f)
            {
                bCanBeStunned = true;
                windowTimer = stunInvulWindow;
                Debug.Log("Can be stunned again");
            }
        }

        //This is only used to test the stun in instances where a player or weapon is not in the scene
        if(bUseTestingHotkey && Input.GetKeyDown(KeyCode.F5))
        {
            StunEnemy(testStunDurtaion);
        }
    }

    public void StunEnemy(float duration)
    {
        if(!IsStunned && bCanBeStunned) //If the enemy is not stunned but can be then run the logic to begin the stun
        {
            Debug.Log("Stunned");
            bCanBeStunned = false;
            stunDuration = duration;
            IsStunned = true;
        }
    }
}
