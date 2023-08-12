using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CaptainSpawner : MonoBehaviour
{
    [SerializeField] Neutral_Controller[] captainUnits;
    [SerializeField] Base_Controller captainController;
    [SerializeField] float captainSpeed;

    void Start()
    {
        captainController = GetComponent<Base_Controller>();
        captainSpeed = captainController.GetMoveSpeed();
        SpawnUnits();
    }

    void SpawnUnits()
    {
        for(int i=0; i<captainUnits.Length; i++)
        {
            Neutral_Controller unit = Instantiate(captainUnits[i], captainController.transform.position, Quaternion.identity, GameManager.Instance.enemySpawner.transform);
            unit.combat.level = captainController.combat.level;
            unit.SetMoveSpeed(captainSpeed);
            unit.neutralCombat.ignoreCamp = true;
            unit.neutralCaptainTransform = transform;
            unit.hasCaptain = true;
        }
    }
}
