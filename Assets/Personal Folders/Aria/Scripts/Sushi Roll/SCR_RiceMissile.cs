using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_RiceMissile : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float missileSpeed;
    [SerializeField] float missileTime;
    [SerializeField] float fireBuffer = 1f;
    [SerializeField] float seekAmount;

    GameObject player;
    Transform missileTransform;
    Transform playerTransform;
    SCR_PlayerStats playerHealth;
    
    bool bIsSeeking = false;
    bool bStartMoving = false;

    Vector3 velocity;
    Vector3 targetDirection;
    Quaternion targetRotation;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        missileTransform = gameObject.transform;
        playerHealth = player.GetComponent<SCR_PlayerStats>();
        velocity = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        missileTime -= Time.deltaTime;
        if(missileTime < 0)
        {
            Destroy(gameObject);
        }

        if(bStartMoving)
        {
            if(bIsSeeking)
            {
                /*missileTransform.position += missileTransform.forward * missileSpeed * Time.deltaTime;
                missileTransform.LookAt(playerTransform.position);*/

                //This doesn't work because of how unity handles rotation; not the same as Unreal / C++
                /*Vector3 distToTarget = playerTransform.position - missileTransform.position;
                Vector3 targetVelocity = distToTarget;
                Vector3 currentVelocity = velocity;

                targetVelocity.Normalize();
                currentVelocity.Normalize();

                Quaternion targetVelocityRot = Quaternion.Euler(targetVelocity);
                Quaternion currentVelocityRot = Quaternion.Euler(currentVelocity);
                Quaternion newVelocityRot = Quaternion.Slerp(currentVelocityRot, targetVelocityRot, Time.deltaTime * seek);

                velocity = newVelocityRot.eulerAngles;
                velocity *= missileSpeed;*/

                fireBuffer -= Time.deltaTime;
                if(fireBuffer <= 0f)
                {
                    //Calculate the direction the missile needs to rotate to face the player
                    //Lerp between its current angle and the target angle with the time being the desired seeking amount
                    targetDirection = playerTransform.localPosition - transform.localPosition;
                    targetRotation = Quaternion.LookRotation(targetDirection);

                    missileTransform.rotation = Quaternion.SlerpUnclamped(missileTransform.localRotation, targetRotation, seekAmount * Time.deltaTime);

                    //missileTransform.LookAt(playerTransform.position);
                    velocity = missileTransform.forward * missileSpeed;
                }
            }
            else
            {
                fireBuffer -= Time.deltaTime;
                if (fireBuffer <= 0f && fireBuffer > -0.5f)
                {
                    targetDirection = playerTransform.localPosition - transform.localPosition;
                    targetRotation = Quaternion.LookRotation(targetDirection);

                    missileTransform.rotation = Quaternion.SlerpUnclamped(missileTransform.localRotation, targetRotation, seekAmount * Time.deltaTime);
                }

                if(fireBuffer <= 0f)
                {
                    velocity = missileTransform.forward * missileSpeed;
                }
            }

            missileTransform.position += velocity * Time.deltaTime;
        }
    }

    public void Fire(bool seeking)
    {
        //missileTransform.LookAt(playerTransform.position);
        missileTransform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        bIsSeeking = seeking;
        bStartMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damageAmount);
        }
        if (fireBuffer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
