using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroRangeCheck : MonoBehaviour
{
    public bool enemyInAggroRange;
    public bool isAggroed;
    [SerializeField] protected Base_Combat combat;

    //Collision avoidance for nearby allies
    [Header("=== Collision Avoidance ===")]
    public float aggroDistance = 0.4f;
    // public float avoidanceForce = 1f;
    // public float maxAvoidanceForce = 5f;
    [SerializeField] protected List<Transform> nearbyTargets;
    //


    protected virtual void Start()
    {
        if(combat == null) combat = GetComponentInParent<Base_Combat>();
        // HeroesInRange = new List<Hero_Combat>();

        nearbyTargets = new List<Transform>();
    }

    protected virtual void Update()
    {
        UpdateNearbyTargets();
        FindTarget();
    }

    protected virtual void UpdateAggroState()
    {
        isAggroed = nearbyTargets.Count > 0;
    }


    // protected virtual void OnTriggerEnter2D(Collider2D collider)
    // {
    //     var hero = collider.GetComponent<Hero_Combat>();
    //     if(hero == null) return;
    //     HeroesInRange.Add(hero);
    //     if(combat.isAttacking) return;

    //     if(HeroesInRange.Count > 1) combat.SetTarget(GetClosestHero());
    //     else combat.SetTarget(hero.transform);
    //     // if(hero.isAlive)
    //     // {
    //     //     combat.SetTarget(hero.transform); //TODO: reference breaks when enemy is dead
    //     //     // numEnemiesInAggro++;
    //     // } 

    //     // else numEnemiesInAggro--;

    //     UpdateAggroState();
    // }

    // protected virtual void 

    // protected virtual void OnTriggerExit2D(Collider2D collider)
    // {
    //     var hero = collider.GetComponent<Hero_Combat>();
    //     if(hero == null) return;
    //     HeroesInRange.Remove(hero);
    //     // numEnemiesInAggro--;
    //     UpdateAggroState();
    // }

    public virtual void FindTarget()
    {
        // Placeholder to be overridden
        if(combat.isAttacking) return;

        if(nearbyTargets.Count == 0) return;
        combat.SetTarget(GetClosestHero());

        if(nearbyTargets.Count > 1) combat.SetTarget(GetClosestHero());
        else combat.SetTarget(nearbyTargets[0].transform);
    }

    Transform GetClosestHero()
    {
        if(nearbyTargets.Count == 0) return null;
        for(int i=0; i<nearbyTargets.Count; i++)
        {
            if(nearbyTargets[i].transform == null) continue;
            Transform target = nearbyTargets[i].transform;
            float distCheck = Vector3.Distance(transform.position, target.position);
            //Target enemy within range
            if(distCheck <= combat.attackRange) return target;
        }
        return nearbyTargets[0].transform;
    }

    protected virtual void UpdateNearbyTargets()
    {
        nearbyTargets.Clear();
        
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Hero");

        foreach (GameObject targetHero in targets)
        {
            if(targetHero == gameObject) continue; //Ignore self
            
            Hero_Combat hero = targetHero.GetComponent<Hero_Combat>();
            if(hero == null || !hero.isAlive) continue; //Ignore if hero is dead

            //Checking if targets are within aggroRange
            float distance = Vector3.Distance(transform.position, targetHero.transform.position);
            if (distance <= aggroDistance)
            {
                nearbyTargets.Add(targetHero.transform);
            }
        }
        UpdateAggroState();
    }
}
