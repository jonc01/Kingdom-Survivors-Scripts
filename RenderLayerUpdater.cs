using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLayerUpdater : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    void Start()
    {
        if(sr == null) sr = GetComponent<SpriteRenderer>();
    }

    // void Update()
    // {

    // }

    void FixedUpdate()
    {
        //Updates sorting order to keep lower characters in front of those behind/above
        sr.sortingOrder = -(int)(transform.position.y*100); //1000
    }
}
