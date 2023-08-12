using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerAnimatorHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Animator animator; //TODO: ;temp public for mecanim
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Base_Controller baseController;
    [SerializeField] Base_Combat combat;
    Coroutine AttackCO;

    private float lockedtill;
    private int currentState;
    private bool attacking;

    private static int Idle = Animator.StringToHash("Idle");
    private static int Move = Animator.StringToHash("Run");

    private static int Attack = Animator.StringToHash("Attack");
    private static int Attack2 = Animator.StringToHash("Attack2");

    private static int Hurt = Animator.StringToHash("Hurt");
    private static int Death = Animator.StringToHash("Death");
    
    private static int Extra = Animator.StringToHash("Extra");

    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        if(baseController == null) baseController = GetComponent<Base_Controller>();
        if(combat == null) combat = GetComponent<Base_Combat>();
    }

    void Update()
    {
        if (!combat.isAlive)
        {
            if(AttackCO != null) StopCoroutine(AttackCO);
            attacking = false;
        }

        if(attacking) return;

        var state = SetAnimState();

        if (state == currentState) return;
        animator.CrossFade(state, 0, 0);
        currentState = state;
    }

    private int SetAnimState()
    {
        if(!baseController.combat.isAlive) return Death;
        return baseController.isMoving == false ? Idle : Move;
    }

    //Attack anims

    public void PlayAttackAnim(int attackNum, float animTime)
    {
        StopAttackCO();
        attacking = true;

        switch (attackNum)
        {
            case 1:
                animator.Play(Attack, 0, 0);
                break;
            case 2:
                animator.Play(Attack2, 0, 0);
                break;
            default:
                break;
        }

        AttackCO = StartCoroutine(AttackState(animTime));
    }

    void StopAttackCO()
    {
        if (AttackCO != null) StopCoroutine(AttackCO);
        attacking = false;
    }

    IEnumerator AttackState(float duration)
    {
        yield return new WaitForSeconds(duration);
        attacking = false;
        // if (rb.velocity == Vector2.zero) animator.CrossFade(Idle, 0, 0);
        // else animator.CrossFade(Move, 0, 0);
    }
}
