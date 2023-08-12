using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Drop : MonoBehaviour
{
    [SerializeField] int healAmount = 20;

    void OnTriggerEnter2D(Collider2D collider)
    {
        var commander = collider.GetComponent<Commander_Combat>();
        if(commander == null) return;
        commander.GetHeal(healAmount);
        Destroy(gameObject);
    }

    private void DeleteObj()
    {
        Destroy(gameObject);
    }
}
