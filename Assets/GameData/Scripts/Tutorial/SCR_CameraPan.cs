using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CameraPan : MonoBehaviour
{
    public void EndCameraAnimation()
    {
        FindObjectOfType<SCR_TutorialManager>().EndCameraAnimation(gameObject);
    }
}
