using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SpoonPositionMatch : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;
    public bool bMatchPos = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(bMatchPos) gameObject.transform.position = objectToFollow.position;
    }
}
