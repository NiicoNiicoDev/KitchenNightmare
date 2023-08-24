using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_WormProjectile : MonoBehaviour
{
    public int damage = 1;

    [SerializeField] private float speed = 1.0f;

    [SerializeField] private float lifetime = 1.0f;

    [SerializeField] private float seek = 1.0f;

    [SerializeField] private GameObject explosionParticles;

    private GameObject spawnedParticles;

    private float distancePercentage = 0f;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation((player.transform.position + new Vector3(0f, 1f, 0f)) - transform.position);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * seek);
        seek += 2f * Time.deltaTime;
        distancePercentage += (Time.deltaTime * speed) / 100.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<SCR_PlayerStats>().TakeDamage(damage);
        } else if (other.CompareTag("Enemy"))
        {
            return;
        }

        spawnedParticles = Instantiate(explosionParticles, transform.position, transform.rotation);
        spawnedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(spawnedParticles, 1f);
        Destroy(gameObject);
    }
}
