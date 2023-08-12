using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    //Manages objects in the pooler
    public static ObjectPoolManager Instance;
    [Header("-- Setup --")]
    [SerializeField] private int amountToPool = 15;
    [SerializeField] private bool expandable = false; //Allows total amount to increase if needed
    [SerializeField] private GameObject pooledPrefab;
    [Header("-")]
    [SerializeField] private List<GameObject> pooledObjects = new List<GameObject>();

    void Awake() { if (Instance == null) Instance = this; }

    protected void Start()
    {
        //Initialize pool with a specified number of objects
        for(int i=0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(pooledPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i=0; i<pooledObjects.Count; i++)
        {
            //Return object that is pooled and not in use
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        if(!expandable) return null;
        else
        {
            GameObject obj = Instantiate(pooledPrefab, Vector3.zero, Quaternion.identity, transform);
            pooledObjects.Add(obj);
            return obj;
        }
    }
}
