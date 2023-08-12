using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_AggroRangeCheck : AggroRangeCheck
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    // protected override void UpdateAggroState()
    // {
    //     isAggroed = EnemiesInRange.Count > 0;
        
    // }

    // protected override void OnTriggerEnter2D(Collider2D collider)
    // {
    //     var enemy = collider.GetComponent<Enemy_Combat>();
    //     if(enemy == null) return;
    //     EnemiesInRange.Add(enemy);
    //     if(combat.isAttacking) return;
        
    //     FindTarget();
    //     // else combat.SetTarget(enemy.transform);
    //     // else combat.SetTarget(GetClosestEnemy());

    //     // numEnemiesInAggro++;
    //     UpdateAggroState();
    // }

    // protected override void OnTriggerExit2D(Collider2D collider)
    // {
    //     var enemy = collider.GetComponent<Enemy_Combat>();
    //     if(enemy == null) return;
    //     EnemiesInRange.Remove(enemy);
    //     // numEnemiesInAggro--;
    //     UpdateAggroState();
    // }

    public override void FindTarget()
    {
        if(!isAggroed) return;
        if(nearbyTargets.Count > 0) combat.SetTarget(GetClosestEnemy());
        // if(nearbyTargets.Count > 1) combat.SetTarget(GetClosestEnemy());
        else if(nearbyTargets[0] != null) combat.SetTarget(nearbyTargets[0].transform);
    }

    // public void ResetTarget()
    // {
    //     if(isAggroed || EnemiesInRange.Count <= 0) return;
    //     combat.SetTarget(GetClosestEnemy());
    // }

    Transform GetClosestEnemy()
    {
        if(nearbyTargets[0] == null) return null;

        for(int i=0; i<nearbyTargets.Count; i++)
        {
            if(nearbyTargets[i] != null)
            {
                Transform enemy = nearbyTargets[i].transform;
                float distCheck = Vector3.Distance(transform.position, enemy.position);
                //Target enemy within range
                if(distCheck <= combat.attackRange) return enemy;
            }else continue;
        }
        //No enemies in attack range, pick first enemy in aggro range
        return nearbyTargets[0].transform;
    }


    //TESTING =================
    protected override void UpdateNearbyTargets()
    {
        nearbyTargets.Clear();
        // Find all enemy objects in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //TODO: override in Hero_AggroRangeCheck

        // Iterate over each enemy
        foreach (GameObject enemy in enemies)
        {
            if(enemy == gameObject) continue; //Ignore self

            // Check if the enemy is within the avoidance distance
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= aggroDistance)
            {
                nearbyTargets.Add(enemy.transform);
            }
        }
        UpdateAggroState();
    }
}
