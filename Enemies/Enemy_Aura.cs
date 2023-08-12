using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Aura : MonoBehaviour
{
    [Header("- Aura -")]
    [SerializeField] float auraDamageFrequency = 1;
    [SerializeField] float auraDamage = 5;
    [SerializeField] float stopHealDuration = 1;
    [Header("- References -")]
    [SerializeField] private Enemy_Combat combat;
    [SerializeField] float damageTimer;
    [SerializeField] private List<Hero_Combat> heroList;

    void Start()
    {
        if(combat == null) combat = GetComponentInParent<Enemy_Combat>();
        heroList = new List<Hero_Combat>();
        damageTimer = 0;
    }

    void FixedUpdate()
    {
        damageTimer += Time.deltaTime;
        if(damageTimer >= auraDamageFrequency) DamageHeroes();
    }

    private void DamageHeroes()
    {
        damageTimer = 0;
        if(heroList.Count == 0) return;
        for(int i=0; i<heroList.Count; i++)
        {
            // heroList[i].TakeDamage(auraDamage);
            heroList[i].DisableHeal(stopHealDuration);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        heroList.Add(hero);
        hero.ToggleBlockedHealIcon(true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        heroList.Remove(hero);
        hero.ToggleBlockedHealIcon(false);
    }
}
