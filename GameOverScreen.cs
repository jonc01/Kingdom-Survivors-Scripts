using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Commander_Combat captainCombat;
    [SerializeField] HeroPartyManager heroPartyManager;
    [Space(20)]
    [Header("Captain Upgrades")]
    [SerializeField] TextMeshProUGUI captainDesc;

    [Space(20)]
    [Header("Hero Upgrades")]
    [SerializeField] TextMeshProUGUI heroDefenseText;
    [SerializeField] Image[] heroIcons;
    [SerializeField] TextMeshProUGUI[] heroDesc;

    void OnEnable()
    {
        StartCoroutine(GetUpgrades());
    }

    IEnumerator GetUpgrades()
    {
        heroDefenseText.text = "+" + heroPartyManager.GetTotalUpgradedDefense();
        // yield return new WaitForSecondsRealtime(0.1f);

        UpdateCaptainUpgrades();

        int heroPartySize = heroPartyManager.HeroesInParty.Count;
        List<Hero_Combat> heroPartyCopy = heroPartyManager.HeroesInParty;
        yield return new WaitForSecondsRealtime(0.1f);
        for(int i=0; i<heroPartySize; i++)
        {
            //Get Hero Icon, toggle the Image component
            heroIcons[i].sprite = heroPartyCopy[i].heroIcon;
            heroIcons[i].gameObject.SetActive(true);
            //Get Hero Weapon Upgrade
            heroDesc[i].text = "";
            heroDesc[i].text += "Lv." + heroPartyCopy[i].GetWeaponLevel() + " Weapon <br>";
            heroDesc[i].text += heroPartyCopy[i].base_damage + "(+" + heroPartyCopy[i].GetUpgradedDamage().ToString("N0") + ") Damage";
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
    }

    void UpdateCaptainUpgrades()
    {
        captainDesc.text = "";
        captainDesc.text += "Lv." + captainCombat.currentLevel + "   " + captainCombat.GetLeveledDefense().ToString("N1");
        captainDesc.text += "(+" + captainCombat.GetUpgradedDefense().ToString("N0") + ") Defense <br>";
        captainDesc.text += "Heal Aura: +" + captainCombat.base_healAmount + "(+" +captainCombat.GetUpgradedHealAmount().ToString("N0") + ")";
        captainDesc.text += "/" + captainCombat.base_healFrequency + "(" + captainCombat.GetUpgradedHealFreq().ToString("N1") + ")s";
    }
}
