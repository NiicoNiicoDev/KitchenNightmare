using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Kitchen Nightmare/Weapon", menuName = "Weapon")]
//Contains the unique properties of each weapon
public class SCR_WeaponStats : ScriptableObject
{
    //Defines the different weapon attacks
    public enum AttackTypes
    {
        PROJECTILE, AOE
    }

    public enum UpgradeTraits
    {
        DAMAGE, FIRERATE, RANGE
    }

    [Header(header: "See script comments for variable descriptions")]
    //weapon name
    public string weaponName = "Weapon Name";

    //Short description of the weapon
    [TextArea]
    public string weaponDescription = "Description";

    //attack damage in units of health
    public float attackDamage = 1f;
    
    //maximum attacks per level allowed for the weapon
    [Header(header:"If no attack limit, enter 0")]
    public int maximumAttacks = 1;

    //attack rate in h/s
    public float attackRate = 1f;

    //type of attack the weapon does
    public AttackTypes attackType;

    //for bigger upgrades, include the same trait multiple times
    //e.g., including two damage traits will increase the damage by 4 times the original rate (compared to the usual 2) at level 2
    public UpgradeTraits[] upgradeTraits;

    //animation that plays when attacking
    public string attackTriggerName;

    //maximum range of the weapon
    public int maximumRange = 1;

    //the value of the weapon in confidence
    public int weaponValue = 1;

    //cost of each upgrade. Array starts at Level 2 upgrade cost. Leave empty if there are no upgrades
    public int[] upgradeCosts;

    [Header(header:"Texture of weapon at each upgrade level")]
    public Material[] weaponTextures;
}
