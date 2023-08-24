using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AnimationDoneProxy : MonoBehaviour
{
    public void AnimationDone()
    {
        transform.parent.GetComponent<SCR_PauseMenu>().AnimationDone();
    }
}
