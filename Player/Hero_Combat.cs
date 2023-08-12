using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hero_Combat : Base_Combat
{
    [Space(20)]
    [Header("= Hero =")]
    [SerializeField] public Sprite heroIcon;
    [SerializeField] public enum _ClassType{
        Melee, Range, Cavalry
    }
    [Space(10)]
    [Header("- Base Hero Stats -")]
    public float base_maxHP;
    public float base_defense;
    public float base_damage;
    public float base_attackSpeed;
    public float knockbackStrength = .05f;
    [Space(10)]
    [Header("|-- Hero ALT --|")]
    public bool rangedUnit = false;
    public bool delayInitialAltAttack = true; //Set false for Ranged
    public bool areaAltAttack = false;
    [SerializeField] string altAttackDesc = "";
    [Space(10)]
    [Header("= Melee Projectile =")]
    [SerializeField] Transform meleeAltOffset;
    [Space(10)]
    [Header("== Hero Stats ==")]
    [SerializeField] public string heroName = "Hero";
    [SerializeField] public string heroDesc = "";
    [Space(20)]
    public _ClassType ClassType; //Enum dropdown
    public int heroClassType; //int accessed by Shop
    [SerializeField] float reviveTime = 10f;
    [SerializeField] public Hero heroSetup;
    // [SerializeField] public int heroID;
    [SerializeField] private CircleCollider2D collider; //might not need override from base
    [SerializeField] private HeroController heroController;
    [SerializeField] private bool canAttack;
    // [Header("= Hitbox Override =")]
    // [SerializeField] protected bool attackBoxOverlap = false;
    // [SerializeField] protected float hitBoxLength = .2f;
    // [SerializeField] protected float hitBoxHeight = .1f;
    [Space(10)]
    [Header("= Hero Health =")]
    [SerializeField] protected TextMeshProUGUI healthText;
    [SerializeField] protected Slider healthBar;
    [SerializeField] public Transform healthBarTransform;
    [SerializeField] public GameObject healIcon;
    [SerializeField] public GameObject blockedHealIcon;
    [Space(10)]
    [Header("Revive Timer")]
    [SerializeField] float localReviveTimer;
    [SerializeField] public GameObject timerParentObj;
    [SerializeField] TextMeshPro timerNumbers;
    private bool isReviving;
    // public bool canHeal;
    [SerializeField] protected float stopHealTimer;

    void Awake()
    {
        //Set Base stats
        base_maxHP = maxHP;
        base_defense = defense;
        base_damage = damage;
        base_attackSpeed = attackSpeed;
    }

    protected override void Start()
    {
        base.Start();

        if(altAttack != null)
        {
            if(!delayInitialAltAttack) altAttack.addProjectiles = 1;
            if(rangedUnit) altAttack.addProjectiles = 0;
        }

        //
        isReviving = false;
        collider = GetComponent<CircleCollider2D>();
        if(heroController == null) heroController = GetComponent<HeroController>();

        if(heroSetup == null) heroSetup = GetComponent<Hero>();
        // base_maxHP = maxHP;
        // base_defense = defense;
        // base_damage = damage;
        // base_attackSpeed = attackSpeed;

        // maxHP = base_maxHP;
        // defense = base_defense;
        // damage = base_damage;
        // attackSpeed = base_attackSpeed;

        // canHeal = true;

        if(meleeAltOffset == null) meleeAltOffset = attackPoint;

        stopHealTimer = 0;

        //Damage Numbers, disabled for Heroes
        displayDamageNumbers = false;

        canAttack = true;
        heroClassType = (int)ClassType; //Setting public variable after manual set in prefab

        UpdateAllHP();
        if(healthBar != null) healthBarTransform = healthBar.transform;
        if(timerNumbers != null)
        {
            timerNumbers.gameObject.SetActive(false);
            timerParentObj = timerNumbers.gameObject;
        }

    }

    protected override void SetProjectileParent()
    {
        // base.SetProjectileParent();
        if(altAttack == null) return;
        altAttack.projectileParentObj = transform.parent.GetComponent<HeroPartyManager>().heroProjectileParent;
    }

    public virtual void UpdateAllHP()
    {
        if(healthBar == null) return;
        healthBar.maxValue = maxHP;
        UpdateCurrentHP();
        GetHeal(0);
    }

    protected virtual void UpdateCurrentHP()
    {
        if(healthBar == null) return;
        healthBar.value = currentHP;
    }

    public override void AggroCheck()
    {
        // Do nothing, overriding inherited
        // if(aggroRangeCheck == null) return;
        // if(!aggroRangeCheck.isAggroed) return;
    }

    protected override void Update()
    {
        base.Update();
        if(stopHealTimer > 0) stopHealTimer -= Time.deltaTime;

        if(timerNumbers == null) return;
        if(!isReviving) 
        { 
            if(timerParentObj != null) timerParentObj.SetActive(false);
            return; 
        }

        if(localReviveTimer <= 0) return;
        localReviveTimer -= Time.deltaTime;
        timerNumbers.text = localReviveTimer.ToString("N0");
    }

    public override void AttackCheck()
    {
        if(target == null) return;
        // if(isAttacking) return;
        if(!aggroRangeCheck.isAggroed) return;
        if(timeSinceAttack <= attackSpeed) return;

        //Check if enemy in range
        if(DistanceCheck(target.position) > attackRange) return;

        StopAttack(); //Resets Attack coroutine and bool checks
        StartAttack(); //timeSinceAttack = 0; is set here
    }

    protected override IEnumerator Attack()
    {
        targetPos = target.position;
        isAttacking = true;
        controller.canMove = false;
        animator.SetBool("Move", false);
        animator.SetTrigger("Attacking");
        if(attackFXAnim != null) attackFXAnim.StartEffectsAnim();
        
        yield return new WaitForSeconds(attackDelayTime);
        
        if(altAttack != null)
        {
            if(delayInitialAltAttack)
            {
                CheckHit();
                if(areaAltAttack) altAttack.AttackArea(meleeAltOffset.position, attackRotatePoint, damage);
                else altAttack.AttackMelee(meleeAltOffset.position, targetPos, attackRotatePoint, damage);
            }
            else
            {
                if(areaAltAttack) altAttack.AttackArea(meleeAltOffset.position, attackRotatePoint, damage);
                else altAttack.Attack(attackPoint.position, targetPos, damage);
            }
        }
        else CheckHit();
        
        controller.canMove = true;
        yield return new WaitForSeconds(fullAttackTime - attackDelayTime);
        isAttacking = false;
    }

    protected override void CheckHit()
    {
        // base.CheckHit();
        if (attackPoint == null) return;
        CheckHitCircle();
    }

    protected void CheckHitCircle()
    {
        Collider2D[] hitEnemies = 
            Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            var combatScript = enemy.GetComponent<Base_Combat>();
            if(combatScript != null)
            {
                if(playAudioClips != null) playAudioClips.PlayRandomClip();
                combatScript.TakeDamage(damage);//, true, knockbackStrength, transform.position.x);
                combatScript.GetKnockback(attackPoint.position, knockbackStrength);
            }
        }
    }

    public override void Die()
    {
        isAlive = false;
        animator.SetBool("Move", false);
        animator.SetTrigger("Death");
        StartCoroutine(Revive());
    }

    public override void TakeDamage(float damageTaken)
    {
        base.TakeDamage(damageTaken);
        UpdateCurrentHP();
        if(healthText == null) return;
        if(currentHP < 0) currentHP = 0;
        healthText.text = currentHP.ToString("N0") + "/" + maxHP.ToString("N0");
    }

    public virtual void GetHeal(float healAmount)
    {
        if(!isAlive) return;
        // if(!canHeal) return;
        if(stopHealTimer > 0) healAmount /= 3;
        if(healAmount < 0) healAmount = 0; //heal amount shouldn't drop below 0

        if(currentHP < maxHP)
            GameManager.Instance.SpawnHealNumber(healAmount, damageNumberOffset.position);

        // EnableHealIcon();
        currentHP += healAmount;
        if(currentHP > maxHP) currentHP = maxHP;
        UpdateCurrentHP();
        if(healthText == null) return;
        healthText.text = currentHP.ToString("N0") + "/" + maxHP.ToString("N0");
    }

    public void ToggleHealIcon(bool toggle)
    {
        // if(!canHeal) healIcon.SetActive(false);
        if(healIcon == null) return;
        healIcon.SetActive(toggle);
    }

    protected void DisableHealIcon()
    {
        ToggleHealIcon(false);
    }

    protected void EnableHealIcon()
    {
        ToggleHealIcon(true);
        Invoke("DisableHealIcon", .3f);
    }

    public void DisableHeal(float stopDuration = 1)
    {
        stopHealTimer = stopDuration;
    }

    public void ToggleBlockedHealIcon(bool toggle)
    {
        if(blockedHealIcon == null) return;
        blockedHealIcon.SetActive(toggle);
    }

    public void RallyCheck()
    {
        heroController.StartRallyTimer();
    }

    IEnumerator Revive()
    {
        //Start revive timer
        isReviving = true;
        localReviveTimer = reviveTime;
        if(timerParentObj != null) timerParentObj.SetActive(true);
        if(timerNumbers != null) timerNumbers.gameObject.SetActive(true);
        //Toggle collider to prevent moving
        collider.enabled = false;
        yield return new WaitForSeconds(reviveTime - .1f);
        //Revive with half of max HP
        if(timerNumbers != null) timerNumbers.gameObject.SetActive(false);
        currentHP = (maxHP*.5f);
        GetHeal(0); //call to update Displayed health
        animator.SetTrigger("Revive");
        isReviving = false;
        UpdateCurrentHP();
        yield return new WaitForSeconds(.1f);
        collider.enabled = true;
        isAlive = true;
    }

    public string GetAltAttackDesc()
    {
        if(altAttack == null) { Debug.Log("No altAttack for " + name); return null; }
        return altAttackDesc;
    }

    public int GetAltAttackLevel(bool nextLevel = false)
    {
        if(altAttack == null) { Debug.Log("No altAttack for " + name); return 0; }
        
        if(nextLevel) return altAttack.level+1; //Upgrade should display the next level
        else return altAttack.level;
    }

    public virtual void UpgradeAltAttack(int numUpgrades = 1)
    {
        if(altAttack == null) { Debug.Log("No altAttack for " + name); return; }
        altAttack.Upgrade(numUpgrades);
    }

    public virtual int GetWeaponLevel() //Rename of AltAttack
    {
        return altAttack.level;
    }

    public virtual float GetUpgradedDamage()
    {
        return damage - base_damage;
    }

    public virtual void UpgradeDefense(float addDefense)
    {
        defense = base_defense + addDefense;
    }

    protected override void OnDrawGizmos()
    {
        // base.OnDrawGizmos();
        
        if (drawGizmos)
        {
            if (attackPoint == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        }
    }
}
