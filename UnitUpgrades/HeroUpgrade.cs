using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroUpgrade : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] GameObject newUnitDisplay;
    [Header("Referenced")]
    [SerializeField] GameObject heroShopObj;
    [SerializeField] HeroShop heroShop;
    public float baseStat;
    public bool isNewUnit;
    [SerializeField] public Hero_Combat hero; //only upgrading Heroes, enemies scale off level only
    [Header("")]
    [Header("Modified")]
    [SerializeField] public int statIndex;
    [SerializeField] public float statIncrease;
    [SerializeField] public float statDecrease;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] TextMeshProUGUI statDesc;

    //TODO: add this to each Slot Button, assign Hero_Combat component to this, call UpgradeStat on button press

    void Awake()
    {
        if(heroShopObj == null) heroShopObj = GetComponentInParent<HeroShop>().gameObject;
        if(heroShop == null) heroShop = heroShopObj.GetComponent<HeroShop>();
        if(statDesc == null) statDesc = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void SelectUnit()
    {
        if(isNewUnit) AddNewUnit();
        else UpgradeStat();

        heroShopObj.SetActive(false);
    }

    void AddNewUnit()
    {
        if(!isNewUnit) return;
        Transform spawnPos = heroShop.heroPartyManager.captainController.transform;
        Instantiate(hero, spawnPos.position, Quaternion.identity, heroShop.heroPartyManager.transform);
        heroShop.heroPartyManager.UpdateHeroParty();
    }

    public void GetStats() 
    {
        //Called in HeroShop.AddHeroToSlot during setup
        if(hero == null) 
        {
            Debug.Log("No hero script for upgrade.");
            return;
        }

        if(isNewUnit)
        {
            if(newUnitDisplay != null) newUnitDisplay.SetActive(true);
            heroName.text = hero.heroName;
            statDesc.text = hero.heroDesc;
        }
        else
        {
            // statIncrease = Random.Range(.1f, .5f); //10-50%
            if(newUnitDisplay != null) newUnitDisplay.SetActive(false);
            heroName.text = hero.heroName;
            string statName = "";
            
            statIndex = Random.Range(0, 3);

            bool displayStatBoostDesc = true;

            switch(statIndex)
            {
                // case 0:
                //     statIncrease = 1.5f;
                //     statName = "Max HP";
                //     baseStat = hero.base_maxHP;
                //     break;
                case 0:
                    displayStatBoostDesc = false;
                    statDesc.text = "Lv." + hero.GetAltAttackLevel(true) + "<br>" + hero.GetAltAttackDesc();
                    break;
                case 1:
                    statIncrease = 1.5f;
                    statName = "Damage";
                    baseStat = hero.base_damage;
                    break;
                case 2:
                    //ALTATTACK upgrade
                    displayStatBoostDesc = false;
                    statDesc.text = "Lv." + hero.GetAltAttackLevel(true) + "<br>" + hero.GetAltAttackDesc();
                    break;
                // case 4:
                //     displayStatBoostDesc = false;
                //     statDesc.text = "Lv." + hero.GetAltAttackLevel(true) + "<br>" + hero.GetAltAttackDesc();
                //     break;
                // case 5:
                    // statIncrease = 1.5f;
                    // statName = "Move Speed";
                    // baseStat = hero.controller.aggroMoveSpeed; //TODO: will need to add public ref
                    // break;
                default: break;
            }

            if(displayStatBoostDesc) statDesc.text = "+" + ((statIncrease-1) * 100).ToString("N0") + "% " + statName;
        }
    }

    public void UpgradeStat()
    {
        switch(statIndex)
        {
            case 0:
                hero.UpgradeAltAttack(1); 
                break;
            // case 1: hero.defense *= statIncrease; break;
            case 1: hero.damage *= statIncrease; break;
            case 2: 
                // hero.attackSpeed -= (hero.base_attackSpeed*statIncrease);
                // if(hero.attackSpeed < 0) hero.attackSpeed = 0;
                hero.UpgradeAltAttack(1);
                break;
            // case 4:
            //     hero.UpgradeAltAttack(1);
            //     break;
            // case 4: aggroMoveSpeed *= upgradeAmountPercent; break;
            default: break;
        }
    }
}
