using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUpgrade : MonoBehaviour
{
    [Header("References")]
    // [SerializeField] int enemyUpgradeIdx;
    public float statBoostPercent = 1;

    void Start()
    {
        statBoostPercent = 1;
    }
    //Apply Stat upgrades after Scale to Level?

    // public float GetStatUpgrades(int enemyIdx)
    // {
        
    // }

    public float GetStatUpgrades()
    {
        return statBoostPercent;
    }
}
