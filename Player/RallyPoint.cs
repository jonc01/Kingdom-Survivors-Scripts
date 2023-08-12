using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    [Header("Heal Aura")]
    public float healRangeMult = 1;
    [SerializeField] Vector3 base_auraRangeScale;

    [SerializeField] private Commander_Combat combat;
    [SerializeField] float healTimer;
    [SerializeField] private List<Hero_Combat> heroList;

    void Start()
    {
        healRangeMult = 1;
        base_auraRangeScale = transform.localScale;

        if(combat == null) combat = GetComponentInParent<Commander_Combat>();
        heroList = new List<Hero_Combat>();
        healTimer = 0;
    }

    void FixedUpdate()
    {
        healTimer += Time.deltaTime;
        if(healTimer >= combat.healFrequency) HealHeroes();
    }

    public void UpgradeHealRange(float percentIncrease)
    {
        healRangeMult += percentIncrease;
        UpdateHealRange();
    }

    private void UpdateHealRange()
    {
        //Reset scale to default
        transform.localScale = base_auraRangeScale;

        //Set scale to new pickup range scale
        transform.localScale = new Vector3(
            transform.localScale.x * healRangeMult, 
            transform.localScale.y * healRangeMult, 
            transform.localScale.z);
    }

    private void HealHeroes()
    {
        healTimer = 0;
        if(heroList.Count == 0) return;
        for(int i=0; i<heroList.Count; i++)
        {
            heroList[i].GetHeal(combat.healAmount);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        heroList.Add(hero);
        hero.ToggleHealIcon(true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var hero = collider.GetComponent<Hero_Combat>();
        if(hero == null) return;
        heroList.Remove(hero);
        hero.ToggleHealIcon(false);
    }
}
