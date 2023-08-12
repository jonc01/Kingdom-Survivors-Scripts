using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alt_Attack : MonoBehaviour
{
    // [Header("-- Alt Attack Base Setup --")]
    // [SerializeField] protected Transform shootPoint;
    [SerializeField] public Transform projectileParentObj; //Set in Hero_Combat
    [SerializeField] protected ObjectPoolManager projectilePool;
    [Header("- Range Attack Setup -")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] public int addProjectiles = 0;
    [SerializeField] public int level;
    [SerializeField] private bool piercing = false;
    [SerializeField] private float flightSpeed = 2.8f;
    [SerializeField] bool isHero = false;
    [SerializeField] Base_Combat combat;

    protected virtual void Start()
    {
        level = 0;
        if(combat == null) combat = GetComponent<Base_Combat>();
        projectilePool = GeneralPoolManager.Instance.heroRanged;
    }

    public virtual void Upgrade(int numUpgrades)
    {
        level++;
        addProjectiles += numUpgrades;
    }
    
    public virtual void Attack(Vector3 shootPoint, Vector3 targetPos, float damage)
    {
        // FireProjectile(transform.position, targetPos, damage);
        if(addProjectiles > 0) StartCoroutine(FireMultipleProjectiles(shootPoint, targetPos, damage));
        else FireProjectile(shootPoint, targetPos, damage);
    }

    public virtual void AttackMelee(Vector3 shootPoint, Vector3 targetPos, Transform rotatePoint, float damage)
    {
        //Placeholder for override
    }

    public virtual void AttackArea(Vector3 shootPoint, Transform rotatePoint, float damage)
    {
        //Placeholder for override
    }

    private void FireProjectile(Vector3 _spawnPos, Vector3 _targetPos, float damage)
    {
        GameObject firedProjObj;

        if(isHero) {
            firedProjObj = projectilePool.GetPooledObject();
        }else{
            firedProjObj = Instantiate(projectilePrefab, _spawnPos, Quaternion.identity, projectileParentObj);
        }

        Projectile firedProjScr = firedProjObj.GetComponent<Projectile>();

        //Get current position of target before firing projectile
        Vector3 finalTargetPos;
        if(combat.target == null) finalTargetPos = _targetPos;
        else finalTargetPos = combat.target.position;

        firedProjObj.transform.position = _spawnPos;
        firedProjScr.projectileDamage = damage;
        firedProjScr.spawnPos = _spawnPos;
        firedProjScr.targetPos = finalTargetPos;
        firedProjScr.piercing = piercing;
        firedProjScr.flightSpeed = flightSpeed;

        firedProjObj.SetActive(true);
    }

    IEnumerator FireMultipleProjectiles(Vector3 _spawnPos, Vector3 _targetPos, float damage)
    {
        for(int i=0; i<addProjectiles+1; i++) //+1 to projectiles since Ranged starts with 1
        {
            //Reduce damage each iteration
            float damageFalloff = damage - i;
            if(damageFalloff <= 0) damageFalloff = 1;

            //yield return after because all projectiles start in this function
            FireProjectile(_spawnPos, _targetPos, damageFalloff);
            yield return new WaitForSeconds(.1f);
        }
    }
}
