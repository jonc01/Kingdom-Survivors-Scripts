using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPoolManager : MonoBehaviour
{
    public static GeneralPoolManager Instance;

    [Header("Hero Projectiles - Range")]
    public ObjectPoolManager heroRanged;
    // public ObjectPoolManager heroPiercingRanged;

    [Space(10)]
    [Header("Hero Projectiles - Melee")]
    public ObjectPoolManager[] heroMeleePool;

    void Awake() { if (Instance == null) Instance = this; }

    // public GameObject GetRangeProjectile(bool isPiercing = false)
    // {
    //     if(isPiercing)
    //     {
    //         // GameObject obj = heroPiercingRanged.GetPooledObject();
    //         // return obj;
    //         return heroPiercingRanged.GetPooledObject();
    //     }else{
    //         // GameObject obj = heroRanged.GetPooledObject();
    //         // return obj;
    //         return heroRanged.GetPooledObject();
    //     }
    // }

    public Projectile GetRangeProjectile()
    {
        GameObject obj = heroRanged.GetPooledObject();
        Projectile proj = obj.GetComponent<Projectile>();

        return proj;
    }

}
