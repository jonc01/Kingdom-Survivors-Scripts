using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Hero", order = 1)]
public class Hero : MonoBehaviour
{
    public string heroName;
    public int heroID = 0;

    void Awake()
    {
        if(heroID == 0) heroID = Animator.StringToHash(heroName);
    }

    public int GetHeroID()
    {
        return heroID;
    }
}
