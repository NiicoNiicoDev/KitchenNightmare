using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_WasabiProjectile : MonoBehaviour
{
    [SerializeField] float fallSpeed;
    [SerializeField] float fallTime;
    [SerializeField] float playerDamage;
    [SerializeField] GameObject wasabiPea;

    SCR_PlayerStats healthScript;

    int groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        fallTime -= Time.deltaTime;
        if(fallTime <= 0f)
        {
            Destroy(gameObject);
        }

        /*float direction = transform.position.y - fallSpeed * Time.deltaTime;
        Vector3 position = new Vector3(transform.position.x, direction, transform.position.z);
        transform.position = position;*/
        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Debug.Log("Hit Player");
            healthScript = other.GetComponent<SCR_PlayerStats>();
            healthScript.TakeDamage((int)playerDamage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == groundLayer) // Code adapted from Unity Technologies, 2021(a)
        {
            //Debug.Log("Hit Floor");
            SCR_EnemyStats enemy = Instantiate(wasabiPea, transform.position, transform.rotation).GetComponent<SCR_EnemyStats>();
            enemy.bSpawnedByBoss = true;
            Destroy(gameObject);
        }
        //end of adapted code
    }
    
}
