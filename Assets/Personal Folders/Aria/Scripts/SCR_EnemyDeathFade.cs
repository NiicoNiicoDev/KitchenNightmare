using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_EnemyDeathFade : MonoBehaviour
{
    [SerializeField] float fadeOutSpeed = 1f;
    [SerializeField] float fadeOutDelay = 3f;
    [SerializeField] GameObject deathParticles;
    [SerializeField] GameObject spawnParticles;
    bool bIsFadingOut = false;
    bool bIsGrowing = false;
    Transform enemyTransform;
    SCR_EnemyStats enemyStats;
    Vector3 defaultEnemyScale;
    
    // Start is called before the first frame update
    void Awake()
    {
        enemyTransform = gameObject.transform;
        enemyStats = GetComponent<SCR_EnemyStats>();
    }

    public void StartGrowIn()
    {
        if (bIsGrowing) return;
        defaultEnemyScale = enemyTransform.localScale;
        enemyTransform.localScale = Vector3.zero;
        bIsGrowing = true;
        if(spawnParticles) spawnParticles = MonoBehaviour.Instantiate(spawnParticles, enemyTransform.localPosition, enemyTransform.localRotation);
        StartCoroutine(GrowIn());
    }

    public void StartShrinkOut(GameObject enemyObject, SCR_ScoreTracker.EnemyType enemyType)
    {
        if (bIsFadingOut) return;
        //Debug.Log("Starting");
        bIsFadingOut = true;
        StartCoroutine(ShrinkOut(enemyType, enemyObject.transform.position));
    }

    IEnumerator ShrinkOut(SCR_ScoreTracker.EnemyType enemyType, Vector3 enemyPosition)
    {
        while(fadeOutDelay > 0f)
        {
            fadeOutDelay -= Time.deltaTime;
            yield return null;
        }
        //deathParticles.GetComponent<ParticleSystem>().Play();
        deathParticles = MonoBehaviour.Instantiate(deathParticles, gameObject.transform.position, gameObject.transform.rotation);
        while (bIsFadingOut)
        {
            enemyTransform.localScale -= (Vector3.one * fadeOutSpeed) * Time.fixedDeltaTime;
            if(enemyTransform.localScale.magnitude < 0.2f)
            {
                bIsFadingOut = false;
            }
            yield return null;
        }

        if(gameObject.CompareTag("Boss"))
        {
            GameManager.gameManager.LevelEnded();
        }

        if(!enemyStats.bSpawnedByBoss)
        {
            //Debug.Log("Adding to score");
            SCR_ScoreTracker.instance.AddToPlayerScore(enemyType, enemyPosition);
        }
        
        GameManager.gameManager.ResetTimeSinceLastKill();

        //Destroy(deathParticles, 0.5f);
        Destroy(gameObject);
    }

    IEnumerator GrowIn()
    {
        if(spawnParticles)
        {
            spawnParticles.GetComponent<ParticleSystem>().Play();
        }

        while (bIsGrowing)
        {
            enemyTransform.localScale += (Vector3.one * fadeOutSpeed) * Time.fixedDeltaTime;
            if (enemyTransform.localScale.magnitude >= defaultEnemyScale.magnitude)
            {
                enemyTransform.localScale = defaultEnemyScale;
                bIsGrowing = false;
            }
            yield return null;
        }

        Destroy(spawnParticles, 0.5f);
    }
}
