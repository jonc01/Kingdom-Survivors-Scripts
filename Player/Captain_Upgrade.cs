using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Captain_Upgrade : MonoBehaviour
{
    [Header("Referenced")]
    [SerializeField] GameObject heroShopObj;
    [SerializeField] HeroShop heroShop;
    public float baseStat;
    [SerializeField] public Commander_Combat captainCombat;

    [Header("Modified")]
    [SerializeField] public int randUpgrade;
    [SerializeField] public float statIncrease;
    [SerializeField] TextMeshProUGUI statDesc;

    void Awake()
    {
        if(heroShopObj == null) heroShopObj = GetComponentInParent<HeroShop>().gameObject;
        if(heroShop == null) heroShop = heroShopObj.GetComponent<HeroShop>();
        if(statDesc == null) statDesc = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SelectUpgrade()
    {
        UpgradeStat();
        heroShopObj.SetActive(false);
    }

    public void GetRandomUpgrade()
    {
        //Called in HeroShop.AddHeroToSlot during setup

        if(captainCombat.healFrequency <= 0.1)
        {
            randUpgrade = Random.Range(0, 3);
        } 
        else randUpgrade = Random.Range(0, 4);

        //Get random upgrades and values
        switch(randUpgrade)
        {
            case 0: //Increased Max HP
                // statIncrease = RandFloat(10, 50);
                statIncrease = 10;
                statDesc.text = "+" + statIncrease.ToString("N0") + " Defense";
                break;
            case 1: //Heal Aura Amount
                // statIncrease = RandFloat(5, 10);
                statIncrease = 5;
                statDesc.text = "Heal Aura: +" + statIncrease.ToString("N0") + " HP per heal interval";
                break;
            case 2: //Heal Aura Range and Pickup Range
                statIncrease = .2f;
                statDesc.text = "Increased Heal Aura and Pickup Range";
                break;
            case 3: //Heal Aura Frequency
                // statIncrease = RandFloat(.1f, .3f);
                statIncrease = .3f;
                statDesc.text = "Heal Aura: " + (statIncrease * 100).ToString("N0") + "% faster heal frequency";
                break;
            // case 5: //Pickup Range
            //     // statIncrease = .3f;
            //     // statDesc.text = "Increased Pickup Range";
            //     break;
            default: break;
        }
    }

    public void UpgradeStat()
    {
        //Apply current upgrade
        switch(randUpgrade)
        {
            case 0: //Increased Max HP
                captainCombat.defense += statIncrease;
                // captainCombat.UpdateAllHP();
                // captainCombat.GetHeal(0); //Updates health text
                break;
            case 1: //Heal Aura Amount
                captainCombat.healAmount += statIncrease;
                break;
            case 2: //Heal Aura Range and Pickup Range
                captainCombat.healAura.UpgradeHealRange(statIncrease);
                captainCombat.pickupController.UpgradePickupRange(.3f);
                break;
            case 3: //Heal Aura Frequency
                captainCombat.healFrequency -= (captainCombat.base_healFrequency * statIncrease);
                //Maxed out at 0.1
                if(captainCombat.healFrequency <= 0.1) captainCombat.healFrequency = 0.1f;
                break;
            // case 5:
            //     break;
            default: break;
        }
    }

    private float RandFloat(float lower, float upper)
    {
        return Random.Range(lower, upper);
    }
}
