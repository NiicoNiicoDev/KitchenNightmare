using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraHandler : MonoBehaviour
{
    PSM_InputHandler inputHandler;

    [SerializeField] Vector2 mouseScrollDelta;
    // Start is called before the first frame update
    void Start()
    {
        inputHandler = FindObjectOfType<PSM_InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHandler.mouseWheelAction.performed += x => mouseScrollDelta = x.ReadValue<Vector2>();

    }
}
