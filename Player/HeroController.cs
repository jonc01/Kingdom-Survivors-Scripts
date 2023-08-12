using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : Base_Controller
{

    [Header("= Hero References =")]
    [SerializeField] HeroPartyManager heroPartyManager;
    [SerializeField] PlayerController captainController;
    [SerializeField] Hero_Combat heroCombat; //override for reference
    [SerializeField] Transform attackFXTransform;
    [Header("= Hero =")]
    public float heroFollowSpeed = 1f; //separate moveSpeed variable to override default
    // [SerializeField] float rallyRange = .45f;
    public bool rallying;
    // public bool canMove;
    [SerializeField] private float baseRallyRange = 0.05f;
    [SerializeField] private float rallyRange = 0.05f;
    [SerializeField] private float aggroRallyRange = .4f;
    public bool inRallyRange;
    // [SerializeField] float aggroMoveSpeedMult = 1.5f; //How fast the Hero runs to rally
    [SerializeField] public float aggroMoveSpeedMult = 1.5f; //.8f
    // [SerializeField] public float rallyMoveSpeed; //Should match captain movespeed
    [SerializeField] private float ignoreRallyTimer;
    [SerializeField] private float rallyDuration = 1;
    protected Transform healthBarTransform;
    protected Transform combatReviveTimerParentObj;
    protected Transform blockedHealParentObj;
    protected float base_moveSpeed;

    protected override void Start()
    {
        base.Start();
        base_moveSpeed = moveSpeed;
        moveSpeed = heroFollowSpeed;
        ignoreRallyTimer = 0;
        rallyRange = baseRallyRange;
        if(heroCombat == null) heroCombat = GetComponent<Hero_Combat>();
        if(heroPartyManager == null) heroPartyManager = GetComponentInParent<HeroPartyManager>();
        if(captainController == null) captainController = heroPartyManager.captainController;
        
        Invoke("GetCaptainRally", .1f);
        healthBarTransform = heroCombat.healthBarTransform;
        attackFXTransform = combat.attackFXTransform;
    }

    void GetCaptainRally() //Only for ref setup
    {
        //Attempt to get rally transforms, retry again until rallyPoint is found
        if(heroPartyManager.GetRallyPoint(heroCombat) == null) Invoke("GetCaptainRally", .2f);
        else
        {
            captainTransform = heroPartyManager.GetRallyPoint(heroCombat);
            heroFollowSpeed = captainController.moveSpeed;// * .8f;
        }

        if(heroCombat.timerParentObj != null) combatReviveTimerParentObj = heroCombat.timerParentObj.transform;
        if(heroCombat.blockedHealIcon != null) blockedHealParentObj = heroCombat.blockedHealIcon.transform;
    }

    protected override void Update()
    {
        if(GameManager.Instance.gamePaused) return;
        if(!combat.isAlive) return;
        Flip();
        // base.Update();

        if(captainTransform == null) return;

        CaptainInRange(); //returns a bool if the Captain is in range

        MoveSpeedCheck();

        
        if(!inRallyRange && ignoreRallyTimer > 0) ignoreRallyTimer -= Time.deltaTime; 

        if(!inRallyRange && ignoreRallyTimer <= 0)
        {
            // if(combat.isAttacking) return;
            if(!canMove) return;
            RallyToCaptain(); //Not in Rally range, rally to captain
            return;
            //lower moveSpeed while (!isAggroed) and Rallying (in function)
        }

        animator.SetBool("Move", false);
        
        if(combat.target == null) combat.aggroRangeCheck.FindTarget();
        //Hero is aggroed, Chase target
        combat.AttackCheck(); //Checks target null, checks aggro, checks isAttacking, checks attackRange
        if(combat.aggroRangeCheck.isAggroed) ChaseTarget(combat.target);

        if(combat.target != null)
            if(combat.target.transform == null) combat.target = null; //Should fix missing Transform
    }

    protected override void FixedUpdate()
    {
        if(!combat.isAlive) return;
        if(!inRallyRange && ignoreRallyTimer <= 0) return;
    }

    private void MoveSpeedCheck()
    {
        if(!inRallyRange)
        {
            if(!combat.aggroRangeCheck.isAggroed)
            {
                moveSpeed = heroFollowSpeed;
            }
            else
            {
                moveSpeed = (base_moveSpeed * aggroMoveSpeedMult);
            }
        }

        if(!combat.aggroRangeCheck.isAggroed)
        {
            rallyRange = baseRallyRange;
        }
        else{
            rallyRange = aggroRallyRange;
        }
    }

    private bool CaptainInRange()
    {
        float distToCaptain = Vector3.Distance(transform.position, captainTransform.position);
        inRallyRange = distToCaptain <= rallyRange;
        // return distToCaptain < rallyRange;
        if(inRallyRange) StartRallyTimer();
        return inRallyRange;
    }

    protected virtual void RallyToCaptain()
    {
        if(!combat.isAlive) return;

        // if(!canMove) return;
        // if(inRallyRange) return;

        FaceTarget(captainTransform.position);

        // float distanceToCaptain = Vector3.Distance(transform.position, captainTransform.position);
        // if(distanceToCaptain < 0.4f) return; //stop chasing if too close
        if(CaptainInRange()) return; 
                
        // heroFollowSpeed = captainController.moveSpeed * .9f;

        
        
        // combat.chasingCaptain = true;
        animator.SetBool("Move", true);
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, captainTransform.position, step);
        
    }

    public void StartRallyTimer()
    {
        if(combat.aggroRangeCheck.isAggroed) ignoreRallyTimer = rallyDuration;
        //No longer needs to rally for 1s if aggroed, after X seconds runs back to rally
    }

    protected override void Flip()
    {
        // base.Flip();
        combat.attackRotatePoint.localScale = transform.localScale;
        if(combat.isAttacking) return;
        // if(healthBarTransform == null) return;
        if(isFacingRight) //Face right
        {
            transform.localScale = new Vector3(defaultScale.x*1f, defaultScale.y, 1);
            healthBarTransform.localRotation = Quaternion.Euler(0, 0, 0);
            if(attackFXTransform != null) attackFXTransform.localRotation = Quaternion.Euler(0, 0, 0);
            if(combatReviveTimerParentObj != null) combatReviveTimerParentObj.localRotation = Quaternion.Euler(0, 0, 0);
            if(blockedHealParentObj != null) blockedHealParentObj.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if(!isFacingRight)//Face left
        {
            transform.localScale = new Vector3(defaultScale.x*-1f, defaultScale.y, 1);
            healthBarTransform.localRotation = Quaternion.Euler(0, 180, 0);
            if(attackFXTransform != null) attackFXTransform.localRotation = Quaternion.Euler(180, 0, 0);
            if(combatReviveTimerParentObj != null) combatReviveTimerParentObj.localRotation = Quaternion.Euler(0, 180, 0);
            if(blockedHealParentObj != null) blockedHealParentObj.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // var heroAlly = collider.GetComponent<Transform>();
        // if(!heroAlly.CompareTag("Hero")) return;
        // nearbyAllies.Add(heroAlly);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // var heroAlly = collider.GetComponent<Transform>();
        // if(!heroAlly.CompareTag("Hero")) return;
        // nearbyAllies.Remove(heroAlly);
    }
}
