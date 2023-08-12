using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Combat : MonoBehaviour
{
    [Header("--- Stats ---")]
    public float level;
    public float maxHP = 10;
    public float currentHP;
    public float defense = 0;
    public bool knockbackImmune = false;
    public float damage = 2;
    public float attackSpeed = .5f;
    [SerializeField] protected int xpAmountDropped = 1;
    [SerializeField] public enum _XPtype {_1, _2, _5, _10};
    public _XPtype XPtype;

    [SerializeField] protected float timeSinceAttack;
    [SerializeField] public float attackRange = .2f;

    [Space(10)]
    [Header("--- Status ---")]
    public bool isAttacking;
    public bool isAlive;
    public Transform target;
    protected Vector3 targetPos; //temp reference in case an enemy dies, reference isn't broken
    public bool chasingCommander; //TODO: does nothing
    [Space(10)]
    [Header("--- Animation Times ---")]
    [SerializeField] protected float fullAttackTime;
    [SerializeField] protected float attackDelayTime;
    [Space(10)]
    [Header("--- References ---")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected AttackEffectsAnimator attackFXAnim;
    public Transform attackFXTransform;
    [SerializeField] protected Transform damageNumberOffset;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] public AggroRangeCheck aggroRangeCheck;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] public Transform attackRotatePoint;
    [SerializeField] protected float attackRadius = .12f;
    [SerializeField] protected bool drawGizmos = false;
    protected Base_Controller controller;
    protected SpriteRenderer sr;
    [SerializeField] protected Material mWhiteFlash;
    protected Material mDefault;
    Coroutine AttackCO;
    public bool isStunned = false;
    [Space(10)]
    [Header("--- Audio ---")]
    [SerializeField] protected PlayAudioClips playAudioClips;
    [Header("|-- ALT Attack override (optional) --|")]
    [SerializeField] protected Alt_Attack altAttack;
    protected bool displayDamageNumbers = true;

    protected virtual void Start()
    {
        ScaleStatsToLevel();
        isStunned = false;
        isAlive = true;
        currentHP = maxHP;
        if(sr == null) sr = GetComponent<SpriteRenderer>();
        mDefault = sr.material;
        if(animator == null) animator = GetComponent<Animator>();
        isAttacking = false;

        SetProjectileParent();

        // attackTimer = 0;
        timeSinceAttack = 0;
        controller = GetComponent<Base_Controller>();
        chasingCommander = false; //TODO: does nothing, might be able to remove safely
        if(attackFXAnim != null && attackFXTransform == null) attackFXTransform = attackFXAnim.transform;

        xpAmountDropped = (int)XPtype;
    }

    protected virtual void SetProjectileParent()
    {
        if(altAttack == null) return;
        altAttack.projectileParentObj = transform.parent;
    }

    protected virtual void Update()
    {
        if(!isAlive) return;
        if(isStunned) return;

        if(isAttacking) return;
        timeSinceAttack += Time.deltaTime;

        AggroCheck();
        if(target != null) AimAtTarget(target.position);
    }

    protected virtual void ScaleStatsToLevel()
    {
        if(level > 0)
        {
            // maxHP = GetScaledStat(maxHP, level, 2.6f);
            // damage = GetScaledStat(damage, level, 1.8f);
            // maxHP = GetScaledStat(maxHP, level, 1f);
            // damage = GetScaledStat(damage, level, 1f); //1.1
            // maxHP += (level-1) * 12;
            maxHP = GetScaledStat(maxHP, level, 1.03f);
            damage += (level-1) * damage;
            //Keeping damage scaling lower, higher HP is the biggest threat since Player gets overrun
        }
    }

    protected float GetScaledStat(float stat, float level, float powNum)
    {
        // float levelScale = stat + (stat * Mathf.Pow(level-1, powNum));
        // float levelScale = stat + (Mathf.Pow((level-1), powNum));

        float levelScale = stat * (Mathf.Pow(level, powNum));

        return levelScale;
    }

    public virtual void SetTarget(Transform targetObj)
    {
        if(targetObj == null) return;
        target = targetObj;
        // if(target != null) 
        // else targetPos = targetObj.position; //TESTING
    }

    public virtual void AggroCheck()
    {
        if(aggroRangeCheck == null) return;
        if(!aggroRangeCheck.isAggroed)
        {
            SetTarget(controller.captainTransform);
            if(DistanceCheck(target.position) > attackRange) return;
            StartAttack();
        } 
    }

    public virtual void AttackCheck()
    {
        if(target == null) return;
        if(isAttacking) return; 
        if(!aggroRangeCheck.isAggroed) return;
        if(timeSinceAttack <= attackSpeed) return;

        //Check if enemy in range
        if(DistanceCheck(target.position) > attackRange) return;

        StopAttack();
        StartAttack();
    }

    protected virtual void StartAttack()
    {
        timeSinceAttack = 0;
        AttackCO = StartCoroutine(Attack());
    }

    protected virtual void StopAttack()
    {
        if (AttackCO != null) StopCoroutine(AttackCO);
        isAttacking = false;
        controller.canMove = true;
    }

    protected float DistanceCheck(Vector3 targetPosition)
    {
        float distToTarget = Vector3.Distance(transform.position, target.position);
        return distToTarget;
    }

    protected void AimAtTarget(Vector3 targetPosition)
    {
        // targetPos = targetPosition;
        Vector3 aimDirection = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        attackRotatePoint.eulerAngles = new Vector3(0, 0, angle);
    }

    protected virtual IEnumerator Attack()
    {
        targetPos = target.position;
        isAttacking = true;
        controller.canMove = false;
        animator.SetBool("Move", false);
        animator.SetTrigger("Attacking");
        if(attackFXAnim != null) attackFXAnim.StartEffectsAnim();
        
        yield return new WaitForSeconds(attackDelayTime);
        
        if(altAttack != null) altAttack.Attack(attackPoint.position, targetPos, damage);
        else CheckHit();
        
        yield return new WaitForSeconds(fullAttackTime - attackDelayTime);
        controller.canMove = true;
        isAttacking = false;
    }

    protected virtual void CheckHit()
    {
        if (attackPoint == null) return;
        Collider2D[] hitEnemies = 
            Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        //knockbackStrength = 6; //TODO: set variable defintion in Inspector

        foreach (Collider2D enemy in hitEnemies)
        {
            var combatScript = enemy.GetComponent<Base_Combat>();
            if(combatScript != null)
            {
                if(playAudioClips != null) playAudioClips.PlayRandomClip();
                combatScript.TakeDamage(damage);//, true, knockbackStrength, transform.position.x);
            }
        }
    }

    public virtual void TakeDamage(float damageTaken)
    {
        if(!isAlive) return;
        HitFlash();
        float totalDamage = damageTaken;
        totalDamage -= defense;
        if(totalDamage < 1) totalDamage = 1;
        currentHP -= totalDamage;

        if(displayDamageNumbers) GameManager.Instance.SpawnDamageNumber(totalDamage, damageNumberOffset.position);

        if(currentHP <= 0) Die();
    }

    public virtual void GetKnockback(Vector3 kbPos, float knockbackStrength)
    {
        if(knockbackImmune) return;
        if(isStunned) return;
        
        Vector3 knockbackDirection = (transform.position - kbPos).normalized;

        transform.position += knockbackDirection * knockbackStrength;
        isStunned = true;
        StartCoroutine(KnockbackCO());
    }

    protected IEnumerator KnockbackCO(float kbDuration = .3f)
    {
        yield return new WaitForSeconds(kbDuration);
        isStunned = false;
    }

    protected virtual void HitFlash()
    {
        sr.material = mWhiteFlash;
        Invoke("ResetMaterial", .1f);
    }

    protected virtual void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public virtual void Die()
    {
        isAlive = false;
        GameManager.Instance.AddScore(xpAmountDropped);
        GameManager.Instance.SpawnXP(transform.position, xpAmountDropped);

        Invoke("DeleteObj", .1f); //TODO: lower delay for explosion
    }

    void DeleteObj()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (attackPoint == null) return;

            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
