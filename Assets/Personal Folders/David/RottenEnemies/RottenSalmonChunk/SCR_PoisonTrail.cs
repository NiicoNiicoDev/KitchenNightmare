using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PoisonTrail : MonoBehaviour
{
    [SerializeField] private GameObject trailCollider;

    //how often the collision is updated. Higher values means better accuracy, but could create FPS drops
    [SerializeField] private float updateFrequency;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PoisonTrail());
    }

    IEnumerator PoisonTrail()
    {
        while (true)
        {
            Instantiate(trailCollider, transform.position, transform.rotation);

            yield return new WaitForSeconds(updateFrequency);
        }
    }
}
