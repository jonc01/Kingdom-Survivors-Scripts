using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Controller : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] public Transform captainTransform;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Base_Combat combat;
    [Space(10)]
    [SerializeField] protected float moveSpeed = 0.5f;
    [SerializeField] protected Animator animator;
    // [Header("- Action Timer -")]
    // [SerializeField] protected float actionDelay = .1f; //
    // [SerializeField] public float actionDelayTimer;

    public bool canMove;
    // public bool canAttack;
    protected Vector3 defaultScale;

    [Header("Movement")]
    [SerializeField] public bool isMoving;
    [SerializeField] protected bool isFacingRight;

    //Collision avoidance for nearby allies
    [Header("=== Collision Avoidance ===")]
    public float avoidanceDistance = .15f;
    public float avoidanceForce = .2f;
    public float maxAvoidanceForce = .2f;
    private List<Transform> nearbyEnemies;
    //

    protected virtual void Start()
    {
        if(captainTransform == null) captainTransform = GameManager.Instance.PlayerTransform;
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        if(combat == null) combat = GetComponent<Base_Combat>();
        if(animator == null) animator = GetComponent<Animator>();
        defaultScale = transform.localScale;
        canMove = true;
        // canAttack = false;
        // actionDelayTimer = 0;
        nearbyEnemies = new List<Transform>();
    }

    protected virtual void Update()
    {
        if(GameManager.Instance.gamePaused) return;
        if(!combat.isAlive) return;
        if(combat.isStunned) return;
        Flip();

        UpdateNearbyEnemies();
        AvoidNearbyAllies();

        if(captainTransform == null) return;

        combat.AttackCheck();

        if(!canMove) return;

        if(combat.target != null) ChaseTarget(combat.target);
        else ChaseTarget(captainTransform);
    }

    protected virtual void FixedUpdate()
    {
        if(!combat.isAlive) return;
        // if(combat.isAttacking) return;
    }


    private void UpdateNearbyEnemies()
    {
        nearbyEnemies.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if(enemy == gameObject) continue; //Ignore self
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= avoidanceDistance)
            {
                nearbyEnemies.Add(enemy.transform);
            }
        }
    }

    private void AvoidNearbyAllies()
    {
        // Find nearby allies
        foreach (Transform enemy in nearbyEnemies)
        {
            // Calculate the desired avoidance direction
            Vector3 avoidanceDirection = transform.position - enemy.position;

            // Apply a force to avoid the enemy
            Vector3 avoidanceForceVector = avoidanceDirection.normalized * avoidanceForce;
            avoidanceForceVector = Vector3.ClampMagnitude(avoidanceForceVector, maxAvoidanceForce);
            transform.position += avoidanceForceVector * Time.deltaTime;
        }
    }


    protected virtual void ChaseTarget(Transform target)
    {
        if(!canMove){ animator.SetBool("Move", false); return; }
        // if(!canMove){ isMoving = false; return;}

        if(target == null || combat.isAttacking)
        {
            animator.SetBool("Move", false);
            return;
        }

        FaceTarget(target.position);
        
        if(DistanceCheck(target.position) > combat.attackRange)
        {
            //Target is in attack range, stop moving
            // animator.SetBool("Move", false);
            
            // return;
            
            //Target is out of range, chase
            animator.SetBool("Move", true);
            var step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    protected float DistanceCheck(Vector3 targetPosition)
    {
        // float distToTarget = Vector3.Distance(transform.position, targetPosition);
        float distToTarget = Vector3.Distance(targetPosition, transform.position);
        return distToTarget;
    }

    protected void FaceTarget(Vector3 targetPosition)
    {
        // isFacingRight
        // isFacingRight = targetPosition.x > transform.position.x;
        if(targetPosition.x > transform.position.x) isFacingRight = true;
        if(targetPosition.x < transform.position.x) isFacingRight = false;
    }

    protected void TargetToRight()
    {
        // isFacingRight
        // isMoving
        
    }

    public float GetMoveSpeed() { return moveSpeed; }

    protected virtual void Flip()
    {
        combat.attackRotatePoint.localScale = transform.localScale;

        if(combat.isAttacking) return;
        //Face right
        if(isFacingRight) 
        {
            transform.localScale = new Vector3(defaultScale.x*1f, defaultScale.y, 1);
        }
        //Face left
        if(!isFacingRight) 
        {
            transform.localScale = new Vector3(defaultScale.x*-1f, defaultScale.y, 1);
        }
    }
}
