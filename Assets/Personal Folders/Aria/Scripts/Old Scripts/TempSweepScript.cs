using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSweepScript : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float sweepAngle = 90;
    Transform noriTransform;
    float angleOffset;
    RaycastHit hit;
    Ray ray;


    // Start is called before the first frame update
    void Start()
    {
        noriTransform = GetComponent<Transform>();

        angleOffset = -sweepAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if(angleOffset <= sweepAngle)
        {
            //Code adapted from Owiegand, 2014
            ray.origin = new Vector3(noriTransform.position.x, noriTransform.position.y - 1.5f, noriTransform.position.z);
            ray.direction = Quaternion.AngleAxis(angleOffset, noriTransform.up) * noriTransform.forward;
            //ray.direction = transform.TransformDirection(ray.direction);
            //End of Adapted Code

            Debug.DrawRay(ray.origin, (ray.direction + noriTransform.forward) * 2.5f, Color.green);
            angleOffset++;
        }
        else
        {
            angleOffset = -sweepAngle;
        }

        if(Physics.Raycast(ray, out hit, 5f, playerLayer))
        {
            Debug.Log("Hit " + hit.collider.name);
        }
        
    }
}
