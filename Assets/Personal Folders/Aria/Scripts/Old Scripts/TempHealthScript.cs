using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempHealthScript : MonoBehaviour
{
    [SerializeField] float startingHealth;
    [SerializeField] TextMeshProUGUI healthText;
    
    private float health;

    public float currentHealth
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            healthText.text = "Player Health: " + currentHealth.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthText.text = "Player Health: " + currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
