using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NeutralCamp : MonoBehaviour
{
    [Header("References")]
    
    [SerializeField] float unitMoveSpeed = .3f;
    [SerializeField] GameObject XPdropPrefab;
    [SerializeField] TextMeshPro levelDisplay;
    [SerializeField] TextMeshPro timerDisplay;
    [SerializeField] private GameObject timerObject;
    [SerializeField] Transform campUnitParent;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Neutral_Combat[] campUnits;

    [Header("Stats")]
    [SerializeField] float currLevel = 1;
    [SerializeField] bool isCleared;
    [SerializeField] float respawnDelay = 15;

    [Header("Debugging")]
    [SerializeField] private float respawnTimer;
    [SerializeField] private bool isRespawning;
    [SerializeField] int totalUnitsAlive;


    void Start()
    {
        respawnTimer = 0;
        isCleared = false;
        isRespawning = false;
        currLevel = 0;
        UpdateLevelDisplay();
        timerObject.SetActive(false);

        // SpawnUnits();
        StartCoroutine(RespawningCO());
        totalUnitsAlive = campUnitParent.childCount;
    }

    void Update()
    {

        if(isRespawning && respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
            UpdateTimer();
        }

        // if(isCleared) Cleared();

        // if(isCleared) return;

    }

    public void Cleared()
    {
        if(isRespawning) return;
        if(XPdropPrefab != null) Instantiate(XPdropPrefab, transform.position, Quaternion.identity);
        StartCoroutine(RespawningCO());
    }

    IEnumerator RespawningCO()
    {
        isRespawning = true;
        respawnTimer = respawnDelay;
        timerObject.SetActive(true);
        yield return new WaitForSeconds(respawnDelay);
        timerObject.SetActive(false);
        currLevel++;
        UpdateLevelDisplay();
        SpawnUnits();
        isRespawning = false;
        isCleared = false;
    }

    void SpawnUnits()
    {
        // totalUnitsAlive = 0;
        for(int i=0; i<campUnits.Length; i++)
        {
            Neutral_Combat unit = Instantiate(campUnits[i], spawnPoints[i].position, Quaternion.identity, campUnitParent);
            unit.level = currLevel; 
            unit.neutralController.SetMoveSpeed(unitMoveSpeed);
        }
        totalUnitsAlive = campUnitParent.childCount;
    }

    public void UpdateUnitCount()
    {
        totalUnitsAlive--;
        if(totalUnitsAlive == 0)
        {
            isCleared = true;
            Cleared();
        }
    }

    void UpdateTimer()
    {
        timerDisplay.text = respawnTimer.ToString("N0");
    }

    void UpdateLevelDisplay()
    {
        levelDisplay.text = "Lv." + currLevel;
    }
}
