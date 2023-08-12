using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPDropPoolerManager : MonoBehaviour
{
    // public static XPDropPoolerManager Instance;
    // [Header("-- Setup --")]
    // [SerializeField] private int[] amountToPool = {15, 10, 5, 1};
    // [SerializeField] private bool expandable = false; //Allows total amount to increase if needed
    // [SerializeField] private GameObject[] pooledPrefabs;
    // [Header("-")]
    // [SerializeField] private List<GameObject> pooledXP1 = new List<GameObject>();
    // [SerializeField] private List<GameObject> pooledXP2 = new List<GameObject>();
    // [SerializeField] private List<GameObject> pooledXP3 = new List<GameObject>();
    // [SerializeField] private List<GameObject> pooledXP4 = new List<GameObject>();

    
    // void Awake() { if (Instance == null) Instance = this; }

    // void Start()
    // {
    //     //Initialize pool with a specified number of objects
    //     for(int poolIdx=0; poolIdx < 4; poolIdx++)
    //     {
    //         for(int i=0; i < amountToPool[poolIdx]; i++)
    //         {
    //             //Instantiate and pool the specified object to its pool
    //             GameObject obj = Instantiate(pooledPrefabs[i]);
    //             obj.SetActive(false);

    //             PoolObject(poolIdx, obj);
    //             // pooledObjects.Add(obj);
    //         }
    //     }
    // }

    // private void PoolObject(int poolIndex, GameObject obj)
    // {
    //     switch(poolIndex)
    //     {
    //         case 0:
    //             pooledXP1.Add(obj);
    //             break;
    //         case 1:
    //             pooledXP2.Add(obj);
    //             break;
    //         case 2:
    //             pooledXP3.Add(obj);
    //             break;
    //         case 3:
    //             pooledXP4.Add(obj);
    //             break;
    //         default: break;
    //     }
    // }

    // public GameObject GetPooledObject(int poolIndex, )
    // {
    //     for(int i=0; i<pooledObjects.Count; i++)
    //     {
    //         //Return object that is pooled and not in use
    //         if(!pooledObjects[i].activeInHierarchy)
    //         {
    //             return pooledObjects[i];
    //         }
    //     }
    //     if(!expandable) return null;
    //     else
    //     {
    //         GameObject obj = Instantiate(pooledPrefab);
    //         PoolObject(poolIdx, obj);
    //         // pooledObjects.Add(obj);
    //         return obj;
    //     }
    // }

    // private GameObject GetObjectPool(int poolIndex)
    // {

    // }
}
