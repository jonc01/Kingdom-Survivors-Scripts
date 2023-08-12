using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Commander_Combat : Hero_Combat
{
    [Header("= Commander =")]
    [SerializeField] public float healFrequency = 1f;
    [SerializeField] public float healAmount = 5f;
    public float base_healFrequency;
    public float base_healAmount;
    public PickupController pickupController;
    public RallyPoint healAura;
    [Header("= Commander Health =")]

    [Header("= Commander Leveling =")]
    [SerializeField] GameObject heroShopObj;
    public int currentXP;
    [SerializeField] public int currentLevel;
    [SerializeField] int maxLevel = 20;
    [SerializeField] int xpForNextLevel;
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] Slider xpSlider;
    [SerializeField] private float levelUpDefense;

    protected override void Start()
    {
        isAlive = true;
        currentHP = maxHP;
        if(sr == null) sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;
        if(animator == null) animator = GetComponent<Animator>();
        isAttacking = false;
        currentXP = 0;
        
        if(pickupController == null) pickupController = GetComponentInChildren<PickupController>();
        if(healAura == null) healAura = GetComponentInChildren<RallyPoint>();

        base_healFrequency = healFrequency;
        base_healAmount = healAmount;

        displayDamageNumbers = false;

        // if(healthText == null) healthText = GameManager.Instance.playerHealth;
        // attackTimer = 0;
        // timeSinceAttack = 0;
        // controller = GetComponent<Base_Controller>();
        currentLevel = 1;
        xpForNextLevel = currentLevel * 10;
        LevelText.text = "Lv." + currentLevel;
        xpSlider.maxValue = xpForNextLevel;
        xpSlider.value = currentXP;
        UpdateAllHP();
        levelUpDefense = 0;

        heroShopObj.SetActive(false);
    }

    public override void AttackCheck()
    {
        //do nothing
    }

    protected override IEnumerator Attack()
    {
        return null;
        // return base.Attack();
        // do nothing
    }

    protected override void Update()
    {
        if(!isAlive) return;
        // if(isAttacking) return;
        // timeSinceAttack += Time.deltaTime;
        // AggroCheck();
    }

    // public void AddGold(int goldAdded) //removing Gold
    // {
    //     // GameManager.Instance.AddScore(goldAdded);
    //     // GameManager.Instance.AddGold(goldAdded);
    // }

    public void AddXP(int xp)
    {
        if(!isAlive) return;
        currentXP += xp;
        xpSlider.value = currentXP;
        if(currentXP >= xpForNextLevel)
        {
            int overflowXP = currentXP - xpForNextLevel;
            LevelUp(overflowXP);
        }
    }

    void LevelUp(int overflowXP)
    {
        currentLevel++;
        LevelText.text = "Lv." + currentLevel;
        // if(currentLevel > maxLevel) currentLevel = maxLevel; //removing max level
        xpForNextLevel = currentLevel * 8;
        xpSlider.maxValue = xpForNextLevel;
        currentXP = overflowXP;
        xpSlider.value = currentXP;
        defense += .25f;
        levelUpDefense += .25f;

        heroShopObj.SetActive(true);
    }

    public override void TakeDamage(float damageTaken)
    {
        // base.TakeDamage(damageTaken);
        if(!isAlive) return;
        HitFlash();

        float totalDamage = damageTaken;
        //Reduce damage to 20% of the total amount for Commander/Captain
        // totalDamage /= 5;
        totalDamage -= defense;

        //Damage ceiling to prevent 1 shot
        //Damage cannot be less than 1
        if(totalDamage > 10) totalDamage = 5;
        else if(totalDamage < 1) totalDamage = 1;
        currentHP -= totalDamage;

        if(displayDamageNumbers) GameManager.Instance.SpawnDamageNumber(totalDamage, damageNumberOffset.position);

        //Update Health numbers
        UpdateCurrentHP();
        if(healthText == null) return;
        if(currentHP < 0) currentHP = 0;
        healthText.text = currentHP.ToString("N0") + "/" + maxHP.ToString("N0");

        if(currentHP <= 0) Die();

        if(playAudioClips != null) playAudioClips.PlayRandomClip();
    }

    public override void GetHeal(float healAmount)
    {
        if(!isAlive) return;
        // if(stopHealTimer > 0) return; //Captain ignores Heal blocker

        // EnableHealIcon();
        currentHP += healAmount;
        if(currentHP > maxHP) currentHP = maxHP;
        UpdateCurrentHP();
        if(healthText == null) return;
        healthText.text = currentHP.ToString("N0") + "/" + maxHP.ToString("N0");
    }

    public override void Die()
    {
        // base.Die();
        // animator.SetTrigger("Death"); //set with AnimatorHandler
        isAlive = false;
        GameManager.Instance.GameOver();
    }

    public float GetLeveledDefense()
    {
        return levelUpDefense;
    }

    public float GetUpgradedDefense()
    {
        return defense - levelUpDefense;
    }

    public float GetUpgradedHealAmount()
    {
        return healAmount - base_healAmount;
    }

    public float GetUpgradedHealFreq()
    {
        return healFrequency - base_healFrequency;
    }
}
