using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPartyManager : MonoBehaviour
{
    [SerializeField] public PlayerController captainController;
    [SerializeField] public List<Hero_Combat> HeroesInParty;
    [SerializeField] public int partySize;
    [Header("- Rally Points -")]
    [SerializeField] Transform rallyPointParent;
    [SerializeField] Transform rallyPointPrefab;
    [SerializeField] float rallyDistance = .1f;
    [SerializeField] public List<Transform> rallyPoints;
    [SerializeField] public Transform heroProjectileParent;
    
    [Space(20)]
    [Header("- Hero Management -")]
    [SerializeField] private float totalUpgradedDefense; //


    void Start()
    {
        totalUpgradedDefense = 0;
        HeroesInParty = new List<Hero_Combat>();
        rallyPoints = new List<Transform>();
        UpdateHeroParty();
    }

    public void UpdateHeroParty()
    {
        HeroesInParty.Clear();
        partySize = transform.childCount;
        //Update list of heroes if Heroes are added/removed/replaced
        for(int i=0; i < partySize; i++) //Only add the required amount
        {
            HeroesInParty.Add(transform.GetChild(i).GetComponent<Hero_Combat>());
        }
        
        //Set follow movement speeds
        for(int i=0; i < HeroesInParty.Count; i++)
        {
            var heroController = HeroesInParty[i].GetComponent<HeroController>();
            heroController.heroFollowSpeed = captainController.moveSpeed;
        }

        UpdateAllHeroDefense(0);

        SetRallyPointsTEMP();

        UpdateRallyPoints();
        SetRallyPoints();
    }

    void UpdateRallyPoints()
    {
        partySize = transform.childCount;
        
        //Add new Rally points
        if(rallyPoints.Count < partySize)
        {
            int countDiff = partySize - rallyPoints.Count;

            for(int i=0; i<countDiff; i++)
            {
                //Instantiate new rally points then add to rally List
                Vector3 rallyParent = rallyPointParent.position;
                Transform newRallyPoint = Instantiate(rallyPointPrefab, rallyParent, Quaternion.identity, rallyPointParent);
                rallyPoints.Add(newRallyPoint);
            }
        }

        //Reposition rally points
        for(int i=0; i<rallyPoints.Count; i++)
        {
            Transform currRallyPoint = rallyPoints[i];

            //Rotate rally points around the main rally point
            float angle = i * (360 / partySize);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            currRallyPoint.position = rallyPointParent.position + dir * rallyDistance;
        }
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }

    public Transform GetRallyPoint(Hero_Combat hero)
    {
        if(HeroesInParty.Count == 0) return null;
        int heroIndex = HeroesInParty.IndexOf(hero);
        return rallyPoints[heroIndex];
    }
    
    private void SetRallyPoints()
    {
        for(int i=0; i<partySize; i++)
        {
            HeroesInParty[i].GetComponent<HeroController>().captainTransform = rallyPoints[i];
        }
    }

    private void SetRallyPointsTEMP()
    {
        //Temporary set while rally points are reset, to prevent null references
        for(int i=0; i<partySize; i++)
        {
            HeroesInParty[i].GetComponent<HeroController>().captainTransform = rallyPointParent;
        }
    }

    public void MoveHero(Vector3 newPos)
    {
        int heroPartySize = partySize;
        if(heroPartySize == 0) return;
        for(int i=0; i<heroPartySize; i++)
        {
            // HeroesInParty[i].transform.position = newPos;
            HeroesInParty[i].transform.position = GetRallyPoint(HeroesInParty[i]).position;
        }
    }

    public void WipeParty()
    {
        //Kill Heroes once the Player Captain dies
        if(HeroesInParty.Count == 0) return;
        for(int i=0; i<HeroesInParty.Count; i++)
        {
            var hero = HeroesInParty[i].GetComponent<Hero_Combat>();
            hero.TakeDamage(99999);
        }
    }

    public void UpdateAllHeroDefense(float defenseIncrease)
    {
        totalUpgradedDefense += defenseIncrease;

        for(int i=0; i<HeroesInParty.Count; i++)
        {
            //Add to current defense of all Heroes
            HeroesInParty[i].UpgradeDefense(totalUpgradedDefense);
        }
    }

    public float GetTotalUpgradedDefense()
    {
        return totalUpgradedDefense;
    }
}
