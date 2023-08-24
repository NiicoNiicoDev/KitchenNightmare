using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TrailCollision : MonoBehaviour
{
    #region POISON DETAILS
    [SerializeField] private float poisonFrequency = 1.0f;
    [SerializeField] private int poisonDamage = 1;
    [SerializeField] private int totalHits = 1;
    #endregion

    [SerializeField] private float lifetime = 1.0f;

    private SCR_PoisonMechanics poisonScript;

    // Start is called before the first frame update
    void Start()
    {
        poisonScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PoisonMechanics>();

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !poisonScript.bIsPoisoned)
        {
            poisonScript.StartPoisoning(poisonFrequency, poisonDamage, totalHits);
        }
    }
}
