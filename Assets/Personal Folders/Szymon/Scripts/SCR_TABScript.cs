using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TABScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject recipeMenu;
    [SerializeField] private GameObject TABButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (recipeMenu.activeSelf)
        {
            TABButton.SetActive(false);
        }
        else
        {
            TABButton.SetActive(true);
        }
    }
}
