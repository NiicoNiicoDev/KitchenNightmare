using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_MaterialPassthroughController : MonoBehaviour
{
    [SerializeField] public float targetOpacity;

    GameObject player;
    [SerializeField] RaycastHit[] hits;
    [SerializeField] AnimationCurve opacityCurve;

    [SerializeField] GameObject[] prevObjHit;
    [SerializeField] GameObject[] hitObjects;

    [SerializeField] LayerMask passthroughMask;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
    }

    void Update()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        hits = Physics.SphereCastAll(transform.position, 1, dir, Vector3.Distance(transform.position, player.transform.position) - 1, passthroughMask);

        GetHitObjects();
    }

    void GetHitObjects()
    {
        foreach (RaycastHit h in hits)
        {
            Debug.Log(h.transform.name);
            Material mat = h.transform.GetComponent<Renderer>().material;

            if (!mat.HasInt("_IsPassthrough"))
            {
                return;
            }
                

            if (!h.transform.GetComponent<SCR_MaterialPassthrough>())
                h.transform.AddComponent<SCR_MaterialPassthrough>();

            h.transform.GetComponent<SCR_MaterialPassthrough>().IsHit();
        }
        
        List<GameObject> hitObjectsList = new List<GameObject>();
            
        for (int i = 0; i < hits.Length; i++)
        {
            hitObjectsList.Add(hits[i].transform.gameObject);
        }

        if (hitObjectsList.Count > hits.Length)
        {
            for (int i = hits.Length; i < hitObjectsList.Count; i--)
            {
                hitObjectsList.RemoveAt(i);
            }
        }

        hitObjects = hitObjectsList.ToArray();

        if (hits.Length == 0)
        {
            hitObjects = new GameObject[0];
        }
    }

    public List<GameObject> GetHitObjectsList() 
    {
        return hitObjects.ToList();
    }
}
