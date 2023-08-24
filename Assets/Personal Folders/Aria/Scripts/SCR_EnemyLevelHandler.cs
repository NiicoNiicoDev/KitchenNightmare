using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_EnemyLevelHandler : MonoBehaviour
{
    public int CurrentLevel { get; private set; }

    [Tooltip("Curve for the health modifier values, X = Level, Y = Health Modifier")]
    [SerializeField] private AnimationCurve healthModifierCurve;
    [Tooltip("Curve for the damage modifier values, X = Level, Y = Damage Modifier")]
    [SerializeField] private AnimationCurve damageModifierCurve;

    public void IncreaseLevel()
    {
        CurrentLevel++;
    }

    public void GetHealthMod()
    {
        //Needs to querey the curve based on the current level to get the desired health modifier
        //This health mod can then be added to the enemie's health

    }

    public void GetDamageMod()
    {
        //Needs to querey the curve based on the current level to get the desired damage modifier
        //This is then added to the base damage of an enemy's attack

    }
}
