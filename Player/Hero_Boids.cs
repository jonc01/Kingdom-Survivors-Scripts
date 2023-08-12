using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Boids : MonoBehaviour
{
    /////////////////////////////////////////
    //PLACEHOLDER SCRIPT
    /////////////////////////////////////////


    public bool enemyInAggroRange;
    public LayerMask enemyLayer;
    // public float numEnemiesInAggro;
    public bool isAggroed;
    [SerializeField] List<Hero_Combat> HeroesInRange;
    [SerializeField] protected Base_Combat combat;

    //Collision avoidance for nearby allies
    [Header("=== Collision Avoidance ===")]
    public float avoidanceDistance = .15f;
    public float avoidanceForce = 1f;
    public float maxAvoidanceForce = 5f;
    private List<Transform> nearbyTargets;
    //


    protected virtual void Start()
    {
        if(combat == null) combat = GetComponentInParent<Base_Combat>();
        HeroesInRange = new List<Hero_Combat>();
    }

    protected virtual void UpdateAggroState()
    {
        isAggroed = HeroesInRange.Count > 0;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        HeroesInRange.Add(hero);
        if(combat.isAttacking) return;

        if(HeroesInRange.Count > 1) combat.SetTarget(GetClosestHero());
        else combat.SetTarget(hero.transform);
        // if(hero.isAlive)
        // {
        //     combat.SetTarget(hero.transform); //TODO: reference breaks when enemy is dead
        //     // numEnemiesInAggro++;
        // } 

        // else numEnemiesInAggro--;

        UpdateAggroState();
    }

    public void OverrideAggroToHero()
    {
        
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        HeroesInRange.Remove(hero);
        // numEnemiesInAggro--;
        UpdateAggroState();
    }

    public virtual void FindTarget()
    {
        // Placeholder to be overridden
    }

    Transform GetClosestHero()
    {
        if(HeroesInRange.Count == 0) return null;
        for(int i=0; i<HeroesInRange.Count; i++)
        {
            Transform enemy = HeroesInRange[i].transform;
            float distCheck = Vector3.Distance(transform.position, HeroesInRange[i].transform.position);
            //Target enemy within range
            if(distCheck <= combat.attackRange) return HeroesInRange[i].transform;
        }
        return HeroesInRange[0].transform;
    }
}
