using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Enemy Wave", menuName = "Scriptable Enemy Wave/Enemy Wave")]
public class SO_FixedEnemyWave : ScriptableObject
{
    [Tooltip("The number of waves in the Scriptable Object")]
    public List<Wave> enemyWaves;
}

[System.Serializable]
public struct Wave
{
    [Tooltip("The number of enemies in this wave")]
    public GameObject[] enemiesInWave;

    public Wave(GameObject[] enemiesInWave)
    {
        this.enemiesInWave = enemiesInWave;
    }
}
