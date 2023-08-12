using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyShop : MonoBehaviour
{
    public float statMultiplier = 1; //Default 1

    [Header("Referenced")]
    [SerializeField] GameObject heroShopObj;
    [SerializeField] EnemyUpgrade enemyUpgrade;
    [SerializeField] public Enemy_Spawner enemy_Spawner;
    [SerializeField] Sprite[] enemyIcons;
    [SerializeField] int enemyIndex;

    [Header("Modified")]
    [SerializeField] float statBoostPercent;
    [SerializeField] TextMeshProUGUI scoreMultDesc;
    [SerializeField] TextMeshProUGUI statDesc;
    [SerializeField] float scoreMultiplier;


    
    void Awake()
    {
        Debug.Log("ENEMYSHOP AWAKE");
        if(heroShopObj == null) heroShopObj = GetComponentInParent<HeroShop>().gameObject;
        if(statDesc == null) statDesc = GetComponentInChildren<TextMeshProUGUI>();
        
        enemy_Spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Enemy_Spawner>();
        enemyUpgrade = enemy_Spawner.GetComponent<EnemyUpgrade>();
    }

    // void OnEnable()
    // {
    //     GetRandomUpgrade(); //Called
    // }

    public void SelectUpgrade()
    {
        GameManager.Instance.UpdateScoreMultiplier(scoreMultiplier);
        enemyUpgrade.statBoostPercent += statBoostPercent;
        heroShopObj.SetActive(false);
    }

    public void GetRandomUpgrade()
    {
        // statDesc.text = "+1"

        int rand = Random.Range(0, 3);
        switch(rand)
        {
            case 0: 
                // statDesc.text = "+" + (statUpgrade*100).ToString("N0") + " % Health";
                statDesc.text = "+" + "10%" + " Health to all Enemies";
                scoreMultiplier = .1f;
                statBoostPercent = .1f;
                break;
            case 1: 
                statDesc.text = "+" + "20%" + " Health to all Enemies";
                scoreMultiplier = .2f;
                statBoostPercent = .2f;
                break;
            case 2:
                statDesc.text = "+" + "30%" + " Health to all Enemies";
                scoreMultiplier = .3f;
                statBoostPercent = .3f;
                break;
            default: break;
        }

        scoreMultDesc.text = "+" + (100*scoreMultiplier).ToString("N0") + "% Score Multiplier";
    }
}
