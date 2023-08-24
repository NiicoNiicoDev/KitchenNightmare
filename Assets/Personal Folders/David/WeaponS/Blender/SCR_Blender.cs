using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows the blender to damage the player when used
public class SCR_Blender : MonoBehaviour
{
    [Header(header:"This is the damage to the player, NOT the enemies")]
    [SerializeField] private int damage = 1;

    private SCR_PlayerHealth healthScript;

    private float timer = 0f;

    #region START
    // Start is called before the first frame update
    void Start()
    {
        healthScript = GetComponentInParent<SCR_PlayerHealth>();
    }
    #endregion

    private void Update()
    {
        timer+= Time.deltaTime;
    }

    #region ON BLENDER USE
    //as the blender can only be used once, damaging the player when the blender is destroyed should work
    //make sure the attack limit for the blender is 1, else it won't damage the player until the final attack
    private void OnDestroy()
    {
        if (timer > 1f && healthScript)
        {
            Debug.Log("Blender hit");
            healthScript.Damage(damage);
        }
    }
    #endregion
}
