using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_KnifeProjectile : MonoBehaviour
{
    public List<GameObject> hitObjects = new List<GameObject>();

    public GameObject knifeParent;
    
    private void Awake()
    {
        StartCoroutine(MoveWeapon());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") == true)
        {
            Debug.Log(collision.gameObject.name);
            if (!hitObjects.Contains(collision.gameObject))
            {
                hitObjects.Add(collision.gameObject);
            }
            //hitObjects.Add(collision.gameObject);
            collision.transform.SetParent(transform);
            //ollision.transform.GetComponent<SCR_EnemyStats>().TakeDamage(knifeParent.GetComponent<Weapon_Knife>().Damage());
        }
    }

    IEnumerator MoveWeapon()
    {
        Debug.Log("Move Projectile");
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.up * knifeParent.GetComponent<Weapon_Knife>().AttackRange();

        while (elapsedTime < 0.2f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var item in hitObjects)
        {
            item.GetComponent<SCR_EnemyStats>().TakeDamage(knifeParent.GetComponent<Weapon_Knife>().Damage());
            item.transform.parent = null;
        }
        Destroy(gameObject);
    }
}
