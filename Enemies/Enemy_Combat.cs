using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Combat : Base_Combat
{
    [Header("= Enemy Upgraded Stats =")]
    public EnemyUpgrade enemyUpgrade;
    // public int enemyUpgradeIndex;
    // public float addHealthPercent = 1;
    // public float addDamagePercent = 1;
    [Space(10)]
    [Header("Optional Manual Set")]
    [SerializeField] Enemy_CaptainSpawner captainSpawner;

    protected override void ScaleStatsToLevel()
    {
        base.ScaleStatsToLevel();
        GetUpgradedStats();
    }
    
    protected virtual void GetUpgradedStats()
    {
        //Get Bonuses from Spawner
        if(enemyUpgrade == null) enemyUpgrade = GameManager.Instance.enemySpawner.GetComponent<EnemyUpgrade>();
        maxHP *= enemyUpgrade.GetStatUpgrades();
        currentHP = maxHP;

        //Add bonuses to enemy stats from upgrade after stats are scaled to level
        // maxHP *= addHealthPercent;
        // currentHP = maxHP;

        // damage *= addDamagePercent;

    }
}
