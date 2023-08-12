using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alt_Attack_Area : Alt_Attack
{
    [Header("- Area Attack Setup -")]
    public _MeleeType MeleeType; //Enum dropdown
    [SerializeField] public enum _MeleeType{
        LongRange, CloseRange, Area, Pull
    }
    [Space(10)]
    [SerializeField] GameObject areaPrefab; //remove once pooling is working
    // [SerializeField] public int addProjectiles = 0; //Overriding as a scale up
    Quaternion projectileRotation;
    [SerializeField] float areaScalePerUpgrade = .5f; //1.0f = one upgrade tier is 100% increase
    float attackRadiusScale = 1;
    private Vector3 base_projScale;
    private float base_radius;
    [SerializeField] Vector3 currentScale;
    [SerializeField] float currentRadius;

    protected override void Start()
    {
        // base.Start();
        level = 0;
        projectilePool = GeneralPoolManager.Instance.heroMeleePool[(int)MeleeType];
        ScaleUpAttack();
        GameObject baseProjectile = projectilePool.GetPooledObject();
        base_projScale = baseProjectile.transform.localScale;
        base_radius = baseProjectile.GetComponent<Projectile_Melee>().attackRadius;

        currentRadius = base_radius *  attackRadiusScale;
        currentScale = base_projScale * attackRadiusScale;
    }

    public override void Upgrade(int numUpgrades)
    {
        base.Upgrade(numUpgrades);
        ScaleUpAttack();
    }

    public override void AttackArea(Vector3 shootPoint, Transform rotatePoint, float damage)
    {
        projectileRotation = rotatePoint.rotation;
        StartAreaAttack(shootPoint, damage);
    }

    private void StartAreaAttack(Vector3 shootPoint, float damageDealt)
    {
        //Pooled
        GameObject firedProjectile = projectilePool.GetPooledObject();
        Projectile_Melee firedProjectileScr = firedProjectile.GetComponent<Projectile_Melee>();

        firedProjectile.transform.position = shootPoint;
        firedProjectileScr.attackRadius = currentRadius;
        firedProjectile.transform.localScale = currentScale;

        //Set fixed rotation for next projectile
        firedProjectile.transform.rotation = projectileRotation;
        firedProjectileScr.meleeProjectileDamage = damageDealt;
        // firedProjectileScr.knockbackStrength = 
        
        firedProjectile.SetActive(true); //Pooled
    }

    private void ScaleUpAttack()
    {
        attackRadiusScale = 1;
        float upgradeFalloff = areaScalePerUpgrade;
        //Loop for total number of upgrades with falloff
        for(int i=0; i<addProjectiles; i++) 
        {
            //damage falloff per upgrade, stops at 10% increase
            upgradeFalloff /= (i+1); // 1/(2,3,4,...)
            if(upgradeFalloff < .15f) upgradeFalloff = .15f;

            attackRadiusScale += upgradeFalloff;
        }
        //Set new attack radius scaling
        currentRadius = base_radius *  attackRadiusScale;
        currentScale = base_projScale * attackRadiusScale;
    }
}
