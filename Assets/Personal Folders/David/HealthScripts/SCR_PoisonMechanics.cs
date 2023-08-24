using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this could be integrated into SCR_PlayerStats at a later date but use for now
public class SCR_PoisonMechanics : MonoBehaviour
{
    [SerializeField] private GameObject poisonParticles;

    [HideInInspector] public bool bIsPoisoned = false;

    private WaitForSeconds totalPoisonTime;

    //how many times the poison has damaged the player
    private int totalHits = 0;

    private SCR_PlayerStats healthScript;

    private GameObject spawnedParticles;

    private IEnumerator poisonCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        healthScript = GetComponent<SCR_PlayerStats>();
    }

    public void StartPoisoning(float poisonFrequency, int damagePerHit, int poisonHits)
    {
        poisonCoroutine = PoisonPlayer(poisonFrequency, damagePerHit, poisonHits);

        StartCoroutine(poisonCoroutine);
    }

    public IEnumerator PoisonPlayer(float poisonFrequency, int damagePerHit, int poisonHits)
    {
        bIsPoisoned = true;

        float poisonLength = poisonFrequency * poisonHits;

        totalPoisonTime = new WaitForSeconds(poisonLength);

        spawnedParticles = Instantiate(poisonParticles, transform.position, transform.rotation);
        spawnedParticles.transform.parent = transform;
        spawnedParticles.transform.localPosition = Vector3.zero;
        spawnedParticles.GetComponent<ParticleSystem>().Play();

        healthScript.DamageOverTime(damagePerHit, poisonLength, poisonFrequency);

        yield return totalPoisonTime;

        Debug.Log("Destroy particles in co-routine");

        bIsPoisoned = false;

        totalHits = 0;

        Destroy(spawnedParticles);
    }

    public void RemovePoison()
    {
        healthScript.RemoveDOTS();
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        Destroy(spawnedParticles);
        bIsPoisoned = false;
    }
}
