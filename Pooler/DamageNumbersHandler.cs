using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumbersHandler : MonoBehaviour
{
    public float damageNumber = 1;
    [SerializeField] TextMeshPro textMesh;
    [SerializeField] Animator anim;
    [SerializeField] private float animDuration;

    void OnEnable()
    {
        textMesh.text = damageNumber.ToString("N0");
        // anim.playableGraph
        Invoke("DestroyObject", animDuration);
    }

    void DestroyObject()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
