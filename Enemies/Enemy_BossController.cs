using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BossController : Base_Controller
{
    [Space(20)]
    [Header("- Boss Controller -")]
    [SerializeField] Transform healIcon;

    protected override void Flip()
    {
        combat.attackRotatePoint.localScale = transform.localScale;

        if(combat.isAttacking) return;
        //Face right
        if(isFacingRight) 
        {
            transform.localScale = new Vector3(defaultScale.x*1f, defaultScale.y, 1);
            if(healIcon != null) healIcon.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //Face left
        if(!isFacingRight) 
        {
            transform.localScale = new Vector3(defaultScale.x*-1f, defaultScale.y, 1);
            if(healIcon != null) healIcon.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
