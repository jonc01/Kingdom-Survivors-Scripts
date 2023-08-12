using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alt_Attack_Melee : Alt_Attack
{
    [Header("- Melee Attack Setup -")]
    public _MeleeType MeleeType; //Enum dropdown
    [SerializeField] public enum _MeleeType{
        LongRange, CloseRange, Area, Pull
    }
    // private int meleePoolIndex;
    [Space(10)]
    public bool stationaryProjectile = false;
    [SerializeField] float damageFallOffPerAttack = 1;
    // [SerializeField] GameObject meleePrefab;
    [Header("Debug")]
    [SerializeField] Vector3 stationarySpawnPoint;
    [SerializeField] Vector3 currentSpawnPoint;
    [SerializeField] Quaternion currentRotation;

    // public int totalAddAttacks = 1;
    [SerializeField] float projectileSpawnDelay = .1f;

    protected override void Start()
    {
        // base.Start();
        level = 0;
        projectilePool = GeneralPoolManager.Instance.heroMeleePool[(int)MeleeType];
    }

    public override void Attack(Vector3 shootPoint, Vector3 targetPos, float damage)
    {
        // if(totalAddAttacks == 0) return;
        // StartCoroutine(StartMeleeWave(shootPoint, targetPos, damage));
    }

    public override void Upgrade(int numUpgrades)
    {
        base.Upgrade(numUpgrades);
    }

    public override void AttackMelee(Vector3 shootPoint, Vector3 targetPos, Transform rotatePoint, float damage)
    {
        if(addProjectiles == 0) return;
        stationarySpawnPoint = shootPoint;
        currentSpawnPoint = shootPoint;
        StartCoroutine(StartMeleeWave(currentSpawnPoint, targetPos, rotatePoint, damage));
    }

    //
    
#region Spawn Projectile Forward
    IEnumerator StartMeleeWave(Vector3 shootPoint, Vector3 targetPos, Transform rotatePoint, float damage)
    {
        for(int i=0; i<addProjectiles; i++)
        {
            //Reduce damage each iteration
            float damageFalloff = damage - damageFallOffPerAttack;
            if(damageFalloff <= 1) damageFalloff = 1;

            if(stationaryProjectile)
            {
                SpawnInitialProjectile(stationarySpawnPoint, rotatePoint, damageFalloff);
            }
            else
            {
                if(i > 0) SpawnAddProjectiles(currentSpawnPoint, damageFalloff);
                else SpawnInitialProjectile(currentSpawnPoint, rotatePoint, damageFalloff);
            }

            yield return new WaitForSeconds(projectileSpawnDelay);
        }
    }

    private void SpawnInitialProjectile(Vector3 shootPoint, Transform rotatePoint, float damageDealt)
    {
        //After initial attack, Instantiate multiple attackFX moving from the initial position
        GameObject firedProjectile = projectilePool.GetPooledObject();
        Projectile_Melee firedProjectileScr = firedProjectile.GetComponent<Projectile_Melee>();

        firedProjectile.transform.position = shootPoint;
        firedProjectile.transform.rotation = rotatePoint.rotation;
        firedProjectileScr.meleeProjectileDamage = damageDealt;
        // firedProjectileScr.knockbackStrength = 

        //Set fixed rotation and spawnPosition for next projectile
        firedProjectile.SetActive(true);
        currentRotation = firedProjectile.transform.rotation; //Initial rotation
        currentSpawnPoint = firedProjectileScr.forwardOffset.position;
    }

    private void SpawnAddProjectiles(Vector3 shootPoint, float damageDealt)
    {
        //After initial attack, Instantiate multiple attackFX moving from the initial position
        GameObject firedProjectile = projectilePool.GetPooledObject();
        Projectile_Melee firedProjectileScr = firedProjectile.GetComponent<Projectile_Melee>();

        //Set to previous projectile's forward offset and rotation
        firedProjectile.transform.position = shootPoint; 
        firedProjectile.transform.rotation = currentRotation;

        firedProjectileScr.meleeProjectileDamage = damageDealt;
        //Set for next projectile
        currentSpawnPoint = firedProjectileScr.forwardOffset.position; 
        firedProjectile.SetActive(true);
    }
#endregion
}
