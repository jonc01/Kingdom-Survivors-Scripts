using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral_Controller : Base_Controller
{
    
    [Header("-= Neutral Unit References =-")]
    [SerializeField] public Transform neutralCaptainTransform;
    public Neutral_Combat neutralCombat;
    [SerializeField] float aggroMoveSpeed = 1f;
    [SerializeField] private float baseMoveSpeed;
    public bool hasCaptain = false;
    // private List<Transform> nearbyEnemies;

    protected override void Start()
    {
        base.Start();
        if(neutralCaptainTransform == null) neutralCaptainTransform = transform.parent.parent;
        if(neutralCombat == null) neutralCombat = GetComponent<Neutral_Combat>();
        captainTransform = neutralCaptainTransform;
        baseMoveSpeed = moveSpeed;
    }

    public void SetMoveSpeed(float captainMoveSpeed)
    {
        moveSpeed = captainMoveSpeed;
    }

    public void SetBaseMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;
    }

    public void SetAggroMoveSpeed()
    {
        if(!hasCaptain) return;
        moveSpeed = aggroMoveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        if(combat.target == null) ResetTarget();
        if(neutralCaptainTransform == null) ResetTarget();
    }


    public void ResetTarget()
    {
        Transform player = GameManager.Instance.PlayerTransform;
        combat.target = player;
        captainTransform = player;
        neutralCaptainTransform = player;
    }
}
