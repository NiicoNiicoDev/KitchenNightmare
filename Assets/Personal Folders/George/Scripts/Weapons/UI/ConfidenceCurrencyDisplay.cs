using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfidenceCurrencyDisplay : MonoBehaviour
{
    private static SCR_PlayerStats playerStats;
    private static TMP_Text currencyText;

    private void Awake()
    {
        playerStats = FindObjectOfType<SCR_PlayerStats>();
        currencyText = GetComponent<TMP_Text>();

        if (playerStats != null)
        {
            UpdateConfidenceCurrency();
        }
    }

    public static void UpdateConfidenceCurrency()
    {
        currencyText.text = "Confidence Currency: " + playerStats.ConfidenceCurreny.ToString();
    }
}
