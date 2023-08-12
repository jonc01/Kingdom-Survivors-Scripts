using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectsAnimator : MonoBehaviour
{
    //make this child of attackPoint, easier to install
    [Header("Setup")]
    [SerializeField] Animator animator;
    [SerializeField] int hashedIntName;

    [Header("Anim Times")]
    [SerializeField] float delayAnimTime;
    [SerializeField] float totalAnimTime;

    public void StartEffectsAnim()
    {
        animator.Play(hashedIntName, 0, 0);
    }
}
