using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerController playerController;
    [SerializeField] Base_Combat combat;

    private float lockedtill;
    private int currentState;

    private static int Idle = Animator.StringToHash("Idle");
    private static int Move = Animator.StringToHash("Run");

    private static int Attack = Animator.StringToHash("Attack");
    private static int Attack2 = Animator.StringToHash("Attack2");
    private static int Rally = Animator.StringToHash("Rally");

    private static int Hurt = Animator.StringToHash("Hurt");
    private static int Death = Animator.StringToHash("Death");
    
    private static int Extra = Animator.StringToHash("Extra");

    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        if(playerController == null) playerController = GetComponent<PlayerController>();
        if(combat == null) combat = GetComponent<Base_Combat>();
    }

    void Update()
    {
        var state = SetAnimState();

        if (state == currentState) return;
        animator.CrossFade(state, 0, 0);
        currentState = state;
    }

    private int SetAnimState()
    {
        if (!combat.isAlive) return Death;
        // return rb.velocity == Vector2.zero ? Idle : Move;
        if(playerController.isRallying) return Rally;
        return playerController.movement == Vector2.zero ? Idle : Move;
    }
}
