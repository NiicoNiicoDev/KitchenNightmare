using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Procedural Generation Settings", menuName = "Procedural Generation Settings")]
public class Procedural_Gen_Settings : ScriptableObject
{
    public int seed = 0;
    public bool useRandomSeed = false;
    public bool useTestedSeeds = false;
}
