using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral_Combat : Base_Combat
{
    [Header("= Neutral Units =")]
    [SerializeField] NeutralCamp neutralCamp;
    [Header("Debugging")]
    [SerializeField] public bool ignoreCamp = false;
    public Neutral_Controller neutralController;

    protected override void Start()
    {
        base.Start();
        
        if(!ignoreCamp && neutralCamp == null)
            neutralCamp = transform.parent.parent.GetComponent<NeutralCamp>();
        

        if(neutralController == null) neutralController = GetComponent<Neutral_Controller>();
    }

    public override void AggroCheck()
    {
        // base.AggroCheck();
        if(aggroRangeCheck == null) return;
        if(!aggroRangeCheck.isAggroed)
        {
            neutralController.SetBaseMoveSpeed();
            SetTarget(controller.captainTransform);
            if(target == null) return;
            if(DistanceCheck(target.position) > attackRange) return;
            //Do nothing
            // StartAttack();
        }else{
            neutralController.SetAggroMoveSpeed();
        }
    }

    public override void Die()
    {
        base.Die();
        if(neutralCamp != null)
            neutralCamp.UpdateUnitCount();
    }


}
