using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SpoonStrike : MonoBehaviour
{
    [SerializeField] Vector3 spoonReadyPosition;
    [SerializeField] Vector3 spoonReadyRotation;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] float stunDuration = 2f;

    SCR_PlayerStats healthScript;
    Vector3 defaultSpoonPos;
    Vector3 defaultSpoonRot;
    Collider spoonCollider;
    float damage;

    bool bHasDealtDamage = false;
    bool bHasCheckedForPlayer = false;

    public bool bIsReady { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        spoonCollider = GetComponent<Collider>(); //Gets the collider of the spoon
        healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerStats>();
        defaultSpoonPos = gameObject.transform.localPosition; //Starting position of the spoon
        defaultSpoonRot = gameObject.transform.localRotation.eulerAngles; //Default rotation of the spoon
    }

    public void ReadyStrike(float spoonDamage)
    {
        Debug.Log("Readying Strike");

        damage = spoonDamage; //Sets the spoon's damage, as hit detection is handled here, this is passed from the Strike Attack State script
        spoonCollider.enabled = true; //enables the collider component on the spoon

        
        //gameObject.transform.localPosition = spoonReadyPosition; //Sets the ready position and rotation
        gameObject.transform.localRotation = Quaternion.Euler(spoonReadyRotation);
        

        bIsReady = true; //sets the Is Ready flag to true
        bHasDealtDamage = false; //Sets the Dealt Damage flag to false (the enemy has not done damage)
        bHasCheckedForPlayer = false;
    }

    public void StruckGround()
    {
        Debug.Log("Struck Ground");
        spoonCollider.enabled = false; //Disable the collider
        if (Physics.CheckSphere(transform.position, 2f, playerLayerMask) && !bHasCheckedForPlayer)
        {
            Debug.Log("Player hit in AOE");
            healthScript.StunPlayer(stunDuration, false);
        }
        bHasCheckedForPlayer = true;
    }

    public void EndStrike()
    {
        Debug.Log("Ending Strike");
        spoonCollider.enabled = false; //Disable the collider
        //gameObject.transform.localPosition = defaultSpoonPos; //Reset the position and rotation
        gameObject.transform.localRotation = Quaternion.Euler(defaultSpoonRot);

        bIsReady = false; //Set the Ready flag to false (the spoon is no longer ready
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bIsReady && !bHasDealtDamage) //if the object is tagged as a Player, and the spoon is ready and has not dealt any damage
        {
            Debug.Log("Hit the player!");
            //healthScript = other.GetComponent<SCR_PlayerStats>(); //Gets the healthScript from the player

            healthScript.TakeDamage((int)damage); //reduces the Player's health by the damage value
            bHasDealtDamage = true; //Sets the Dealt Damage flag to true
        }
    }
}
