using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumbersPoolManager : MonoBehaviour
{
    //Manages Damage Numbers objects in the pooler
    public static DamageNumbersPoolManager Instance;
    [Header("-- Setup --")]
    [SerializeField] private int amountToPool = 15;
    [SerializeField] private bool expandable = false; //Allows total amount to increase if needed
    [SerializeField] private DamageNumbersHandler damageNumberPrefab;
    [Header("-")]
    [SerializeField] private List<DamageNumbersHandler> pooledObjects = new List<DamageNumbersHandler>();

    void Awake() { if (Instance == null) Instance = this; }

    void Start()
    {
        //Initialize pool with a specified number of objects
        for(int i=0; i < amountToPool; i++)
        {
            DamageNumbersHandler obj = Instantiate(damageNumberPrefab, transform);
            obj.gameObject.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public DamageNumbersHandler GetPooledObject(float damage, Vector3 position)
    {
        for(int i=0; i<pooledObjects.Count; i++)
        {
            //Return object that is pooled and not in use
            if(!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].damageNumber = damage;
                pooledObjects[i].transform.position = position;
                return pooledObjects[i];
            }
        }
        if(!expandable) return null;
        else
        {
            DamageNumbersHandler obj = Instantiate(damageNumberPrefab, transform);
            pooledObjects.Add(obj);
            obj.gameObject.SetActive(false);
            obj.damageNumber = damage;
            obj.transform.position = position;
            return obj;
        }
    }
}
