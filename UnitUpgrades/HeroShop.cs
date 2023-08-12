using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroShop : MonoBehaviour
{
    [Header("References")]
    // [SerializeField] Commander_Combat captainCombat; 
    [SerializeField] public HeroPartyManager heroPartyManager;
    // HeroPartyManager
    [SerializeField] GameObject heroPartyParentObj;
    [SerializeField] public List<Hero_Combat> heroPartyCopy;
    [SerializeField] List<int> heroPartyCopyID;
    [Header("Manual Setup -")]
    [SerializeField] Hero_Combat[] heroPool; //Setup with Non-upgraded units
    [Space(10)]
    [Header("Upgrade Slots - Manual setup")]
    [SerializeField] GameObject[] heroSlots;
    [SerializeField] Image[] heroSlotsIcons;
    [SerializeField] TextMeshProUGUI[] heroSlotsDesc;
    [SerializeField] HeroUpgrade[] heroUpgrades;
    [SerializeField] GameObject heroAllUpgrade;
    [SerializeField] Captain_Upgrade captainUpgrade;
    [SerializeField] EnemyShop enemyUpgradeShop;
    [Header("Temp Listed Lists")]
    [SerializeField] List<Hero_Combat> tempListedHeroesCopy;
    [SerializeField] List<Hero_Combat> tempListedHeroesDupeCheck;
    private bool populatingShop;
    

    void Start()
    {
        heroPartyCopy = new List<Hero_Combat>();
        heroPartyCopyID = new List<int>();
        tempListedHeroesCopy = new List<Hero_Combat>();
        tempListedHeroesDupeCheck = new List<Hero_Combat>();
        populatingShop = false;
    }

    void OnEnable()
    {
        GameManager.Instance.shopOpen = true;
        GameManager.Instance.PauseGame();
        PopulateUpgradeShop();
    }

    void OnDisable()
    {
        GameManager.Instance.shopOpen = false;
        GameManager.Instance.ResumeGame();
        ToggleHeroSlots(false);
        heroAllUpgrade.SetActive(false);
    }

    void AddHeroToSlot(Hero_Combat hero, int i)
    {
        Sprite heroIcon = hero.GetComponent<SpriteRenderer>().sprite;
        heroSlotsIcons[i].sprite = heroIcon;
        heroUpgrades[i].hero = hero;
        
        int _heroID = hero.heroSetup.heroID;
        if(heroPartyCopyID.Contains(_heroID))
        {
            heroUpgrades[i].isNewUnit = false;

            foreach(Hero_Combat dupeHero in heroPartyCopy)
            {
                if(dupeHero.heroSetup.heroID == _heroID)
                {
                    heroUpgrades[i].hero = dupeHero;
                }
            }
        }
        else heroUpgrades[i].isNewUnit = true;
        
        heroUpgrades[i].GetStats();
        captainUpgrade.GetRandomUpgrade();
        // enemyUpgradeShop.GetRandomUpgrade(); //TODO: temp disable
    }

    public void PopulateUpgradeShop()
    {
        if(populatingShop) return;
        StartCoroutine(PopulateShopCO());
    }

    IEnumerator PopulateShopCO()
    {
        populatingShop = true;
        ToggleHeroSlots(false);

        tempListedHeroesDupeCheck.Clear();
        tempListedHeroesCopy.Clear();
        heroPartyCopyID.Clear();

        yield return new WaitForSecondsRealtime(.02f);

        //Create copies of lists to modify when upgrading or putting Heroes in shop
        heroPartyCopy = new List<Hero_Combat>(heroPartyManager.HeroesInParty);
        tempListedHeroesCopy = new List<Hero_Combat>(heroPool); //modified list to prevent dupe listing

        yield return new WaitForSecondsRealtime(.02f);

        for(int i=0; i<heroPartyCopy.Count; i++)
        { //Getting IDs of heroes to check if New or already in the Party
            heroPartyCopyID.Add(heroPartyCopy[i].heroSetup.heroID);
        }

        yield return new WaitForSecondsRealtime(.02f);
        
        for(int i=0; i<3; i++)
        {
            int randIndex = Random.Range(0, tempListedHeroesCopy.Count);

            //Loop through HeroPool, get 3 random heroes
            Hero_Combat selectedHero = tempListedHeroesCopy[randIndex];

            //Remove any Hero from the 
            tempListedHeroesDupeCheck.Add(selectedHero);
            tempListedHeroesCopy.Remove(selectedHero);
            
            //Upgrade duplicate heroes, recruit new heroes
            AddHeroToSlot(selectedHero, i);
            yield return new WaitForSecondsRealtime(.02f);
        }

        yield return new WaitForSecondsRealtime(.1f);
        ToggleHeroSlots(true);
        heroAllUpgrade.SetActive(true);
        populatingShop = false;
    }

    void ToggleHeroSlots(bool toggle)
    {
        for(int i=0; i<heroSlots.Length; i++)
        {
            heroSlots[i].SetActive(toggle);
        }
        captainUpgrade.gameObject.SetActive(toggle);
        // enemyUpgradeShop.gameObject.SetActive(toggle); //TODO: temp disable
    }
}
