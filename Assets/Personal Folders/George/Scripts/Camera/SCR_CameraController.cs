using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private float camSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camOffset = new Vector3(-5, 6, 5);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        Vector3 targetPos = Vector3.Lerp(transform.position, player.transform.position + camOffset, camSpeed * Time.deltaTime);

        transform.position = targetPos;
    }

    void CameraZoom()
    {

    }
}
