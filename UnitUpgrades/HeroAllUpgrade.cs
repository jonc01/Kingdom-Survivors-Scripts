using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroAllUpgrade : MonoBehaviour
{
    [Header("Referenced")]
    [SerializeField] GameObject heroShopObj;
    [SerializeField] HeroPartyManager heroPartyManager;
    [SerializeField] TextMeshProUGUI statDesc;
    float statIncrease;


    void Awake()
    {
        if(heroShopObj == null) heroShopObj = GetComponentInParent<HeroShop>().gameObject;
        if(statDesc == null) statDesc = GetComponentInChildren<TextMeshProUGUI>();
        if(heroPartyManager == null) heroPartyManager = GameObject.FindGameObjectWithTag("HeroParty").GetComponent<HeroPartyManager>();
    }

    void OnEnable()
    {
        GetStats();
        UpdateDisplay();
        heroPartyManager.UpdateHeroParty();
    }

    public void SelectUpgrade()
    {
        UpgradeStat();
        heroShopObj.SetActive(false);
    }

    void UpgradeStat()
    {
        heroPartyManager.UpdateAllHeroDefense(statIncrease);
    }

    void GetStats()
    {
        float rand = Random.value;

        if(rand < .8f) statIncrease = 1;
        else if(rand < .9f) statIncrease = 2;
        else statIncrease = 3;
    }

    void UpdateDisplay()
    {
        statDesc.text = "+" + statIncrease + " Defense";
    }
}
