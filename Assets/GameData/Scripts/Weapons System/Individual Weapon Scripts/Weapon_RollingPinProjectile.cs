using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_RollingPinProjectile : MonoBehaviour
{
    public GameObject rollingPinParent;
    

    private void Awake()
    {
        StartCoroutine(MoveProjectile());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit Enemy");
            StartCoroutine(Knockback(collision.GetContact(0).normal.normalized * -1, collision.gameObject));
            collision.gameObject.GetComponent<SCR_EnemyStats>().TakeDamage(rollingPinParent.GetComponent<Weapon_RollingPin>().Damage());
        }
    }

    IEnumerator MoveProjectile()
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * rollingPinParent.GetComponent<Weapon_RollingPin>().AttackRange();

        while (elapsedTime < 0.5f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator Knockback(Vector3 knockbackDirection, GameObject hitObject)
    {
        float elapsedTime = 0;
        Vector3 startPos = hitObject.transform.position;
        Vector3 endPos = hitObject.transform.position + knockbackDirection;


        while (elapsedTime < 0.2f)
        {
            hitObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
