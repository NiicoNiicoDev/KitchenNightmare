using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AnimationSetInactive : MonoBehaviour
{
    public void SetInactive()
    {
        transform.parent.GetComponent<SCR_PauseMenu>().DisableQuickAccessMenu();
    }
}
