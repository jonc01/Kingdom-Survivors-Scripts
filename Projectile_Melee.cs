using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Melee : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] bool poolObject = false;
    [SerializeField] public float knockbackStrength = .1f;
    [SerializeField] public float attackRadius = .2f;
    [SerializeField] public Transform forwardOffset;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Animator animator;
    [SerializeField] int animName;
    [SerializeField] float animHitDelay = .2f;
    [SerializeField] float fullAnimTime = .6f;
    [SerializeField] bool showGizmos = false;

    [Header("Set at Instantiate")]
    [SerializeField] public Vector3 spawnPos; //TODO: no use
    [SerializeField] public Vector3 targetPos; //TODO: no use
    [SerializeField] public float meleeProjectileDamage = 1;
    Vector3 moveDir; //TODO: no use

    [Header("Audio")]
    [SerializeField] PlayAudioClips playAudioClips;

    void Start() { StartMelee(); }
    void OnEnable() { StartMelee(); }

    private void StartMelee()
    {
        StartCoroutine(AttackCO());
    }

    IEnumerator AttackCO()
    {
        animator.Play(animName, 0, 0);
        yield return new WaitForSeconds(animHitDelay);
        CheckHitCircle();
        yield return new WaitForSeconds(fullAnimTime - animHitDelay);
        Invoke("DeleteObj", .1f);
    }

    private void CheckHitCircle()
    {
        Collider2D[] hitEnemies = 
            Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            var combatScript = enemy.GetComponent<Base_Combat>();
            if(combatScript != null)
            {
                if(playAudioClips != null) playAudioClips.PlayRandomClip();
                combatScript.TakeDamage(meleeProjectileDamage);
                combatScript.GetKnockback(transform.position, knockbackStrength);
            }
        }
    }

    private void DeleteObj()
    {
        if(poolObject) gameObject.SetActive(false);
        else Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if(showGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}
