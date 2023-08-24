using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SpoonSpin : MonoBehaviour
{
    [SerializeField] Vector3 spoonReadyPosition;
    [SerializeField] Vector3 spoonReadyRotation;

    SCR_PlayerStats healthScript;
    SCR_SpoonPositionMatch spoonPositionMatch;
    Vector3 defaultSpoonPos;
    Vector3 defaultSpoonRot;
    Collider spoonCollider;
    float damage;
    bool bHasDealtDamage = false;

    public bool bIsReady { get; private set; } = false; //property that keeps track of whether of not the spoon is ready to be spun, this needs to be got from other scripts but never needs to be set outside of this one

    // Start is called before the first frame update
    void Start()
    {
        spoonCollider= GetComponent<Collider>(); //Gets the collider of the spoon
        spoonPositionMatch = GetComponent<SCR_SpoonPositionMatch>();
        defaultSpoonPos = gameObject.transform.localPosition; //Starting position of the spoon
        defaultSpoonRot = gameObject.transform.localRotation.eulerAngles; //Default rotation of the spoon
    }

    private void Update()
    {
        if(bIsReady)
        {
            //gameObject.transform.localPosition = spoonReadyPosition; //Sets the ready position and rotation
            gameObject.transform.localRotation = Quaternion.Euler(spoonReadyRotation);
        }
    }

    //Ready the Spin
    public void ReadySpin(float spoonDamage)
    {

        damage = spoonDamage; //Sets the spoon's damage, as hit detection is handled here, this is passed from the Spin Attack State script
        spoonCollider.enabled = true; //enables the collider component on the spoon


        //gameObject.transform.localPosition = spoonReadyPosition; //Sets the ready position and rotation
        gameObject.transform.localRotation = Quaternion.Euler(spoonReadyRotation);


        bIsReady = true; //sets the Is Ready flag to true
        bHasDealtDamage = false; //Sets the Dealt Damage flag to false (the enemy has not done damage)
    }

    //Ends the spin and resets all values back to their default
    public void EndSpin()
    {
        spoonCollider.enabled = false; //Disable the collider
        bIsReady = false; //Set the Ready flag to false (the spoon is no longer ready


        //gameObject.transform.localPosition = defaultSpoonPos; //Reset the position and rotation
        gameObject.transform.localRotation = Quaternion.Euler(defaultSpoonRot);


    }

    //Collision detection
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && bIsReady && !bHasDealtDamage) //if the object is tagged as a Player, and the spoon is ready and has not dealt any damage
        {
            healthScript = other.GetComponent<SCR_PlayerStats>(); //Gets the healthScript from the player

            if(healthScript != null )
            {
                healthScript.TakeDamage((int)damage); //reduces the Player's health by the damage value
            }
            bHasDealtDamage = true; //Sets the Dealt Damage flag to true
            
            EndSpin(); //End the spin
        }
    }
}
