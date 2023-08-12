using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    //



    // Replaced with Enemy_Spawner, temp keeping








    [SerializeField] public bool startSpawning;
    [SerializeField] TextMeshProUGUI stageWaveDisplay;
    [Header("Stage Text Display")]
    [SerializeField] GameObject upcomingStageDisplay;
    [SerializeField] GameObject finalStageDisplay;
    [Header("Timer Display")]
    [SerializeField] GameObject stageTimerObj;
    [SerializeField] TextMeshProUGUI stageTimerDisplay;
    [Space(10)]
    [Header("Scaling")]
    [SerializeField] public float enemyLevel = 1;
    [SerializeField] private float timeDelayNextWave = 1;
    [SerializeField] private float timeDelayNextStage = 5;

    [Space(10)]
    [Header("= Spawns =")]
    [SerializeField] float spawnDistance = 3;
    [SerializeField] float spawnFrequencyTimer = 1f; //0.5f
    [SerializeField] private float spawnTimer;
    [SerializeField] private int spawnCounter;
    [SerializeField] private int currEnemySpawn;
    [Space(10)]
    [Header("= Waves =")]
    [SerializeField] int maxStage = 5;
    [SerializeField] private int currentStage;
    [SerializeField] private int currentWave;
    [SerializeField] bool stagesCleared;
    private float stageTimer;
    [Space(10)]
    [Header("= Enemies to Spawn Each Wave (!-Gets set automatically)=")]
    [SerializeField] private EnemyUpgrade enemyUpgrade;
    // [SerializeField] List<int> currentWaveSpawns;
    [SerializeField] int[] currWaveEnemyIndex;
    [SerializeField] int[] currSpawnsPerEnemyWave;
    [Header("Needs Setup")]
    [SerializeField] GameObject[] enemies;
    // [SerializeField] int[] spawnStarts;
    [Space(10)]
    [Header("- Waves: Enemy Index (Needs Setup) -")]
    [SerializeField] int[] waveEnemyIndex1;
    [SerializeField] int[] waveEnemyIndex2;
    [SerializeField] int[] waveEnemyIndex3;
    [SerializeField] int[] waveEnemyIndex4;
    [SerializeField] int[] waveEnemyIndex5;
    [Header("- Waves: NumSpawnsPerEnemy (Needs Setup) -")]
    [SerializeField] int[] spawnsPerEnemyWave1;
    [SerializeField] int[] spawnsPerEnemyWave2;
    [SerializeField] int[] spawnsPerEnemyWave3;
    [SerializeField] int[] spawnsPerEnemyWave4;
    [SerializeField] int[] spawnsPerEnemyWave5;

    [Space(10)]
    [Header("Spawn Locations")]
    [SerializeField] Transform commanderTransform;
    [SerializeField] Transform spawnPoint;
    private Commander_Combat commanderCombat;

    void Start()
    {
        spawnTimer = spawnFrequencyTimer;
        stageTimer = 0;
        commanderTransform = GameManager.Instance.PlayerTransform;
        commanderCombat = commanderTransform.GetComponent<Commander_Combat>();
        enemyLevel = 1;
        spawnCounter = 0;

        stagesCleared = false;
        currEnemySpawn = 0;
        currentStage = 1;
        currentWave = 0;
        GetWaveSpawns();
        UpdateDisplayText();
        upcomingStageDisplay.SetActive(false);
        finalStageDisplay.SetActive(false);

        if(enemyUpgrade == null) enemyUpgrade = GetComponent<EnemyUpgrade>();
    }

    void Update()
    {
        if(commanderTransform == null) return;
        if(!commanderCombat.isAlive) return;
        if(!startSpawning) return;

        //Spawns
        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0) SpawnEnemy();

        //Stage
        if(stageTimer <= 0)
        {
            ToggleDisplayText(false);
            ToggleTimerText(false);

            if(stagesCleared)
            {
                GameManager.Instance.GameOver();
                return;
            }
            return;
        }else{
            ToggleTimerText(true);
        }
        stageTimer -= Time.deltaTime;
        stageTimerDisplay.text = stageTimer.ToString("N0") + "s";
    }
    
    void SpawnEnemy()
    {
        if(stagesCleared) return; //Stop spawning once all Stages are cleared
        spawnTimer = spawnFrequencyTimer;

        if(currentWave > 4) //currentWave is the index, 0-4
        {
            //New Stage, reset Wave to 1
            currentWave = 0;
            GetWaveSpawns();
            enemyLevel++;
            spawnCounter = 0;
            currEnemySpawn = 0;
            // spawnCounter = spawnsPerWave;

            //Check if Final Wave of Final Stage is done
            if(currentStage >= maxStage)
            {
                Debug.Log("Max Stage/Wave reached, Stop spawns");
                //TODO: call end condition here
                stagesCleared = true;
                stageTimer = 30;
                ToggleDisplayText(true, true); //"Victory in: "
                return;
            }
            currentStage++;

            spawnTimer = timeDelayNextStage;
            stageTimer = timeDelayNextStage;
            ToggleDisplayText(true, false); //"Next Stage in: "
            return;
        }

        //Enemy index to spawn // currWaveEnemyIndex1 = { 1, 2, 3 }
        //Total num to spawn // currSpawnsPerEnemyWave1 = { 30, 10, 10 }
        //index iterators // currEnemySpawn and spawnCounter
        if(spawnCounter >= currSpawnsPerEnemyWave[currEnemySpawn])
        {
            spawnCounter = 0;
            currEnemySpawn++;
            if(currEnemySpawn >= currSpawnsPerEnemyWave.Length)
            {
                spawnTimer = timeDelayNextWave;
                currentWave++;
                // waveTimerToggle.SetActive(true);
                spawnCounter = 0;
                currEnemySpawn = 0;
                GetWaveSpawns();
                return;
            }
        }
        // else waveTimerToggle.SetActive(false);

        UpdateDisplayText();

        //Get index of current enemy in wave
        int enemyIdx = currWaveEnemyIndex[currEnemySpawn];
        GameObject enemyToSpawn = enemies[enemyIdx];
        Enemy_Combat enemyCombat = enemyToSpawn.GetComponent<Enemy_Combat>();
        enemyCombat.level = enemyLevel;
        enemyCombat.enemyUpgrade = enemyUpgrade;
        //Get Spawn position, then spawn
        Transform randSpawnPos = GetRandSpawnPosition();
        Instantiate(enemyToSpawn, randSpawnPos.position, Quaternion.identity, transform);

        spawnCounter++;
    }

    private void GetWaveSpawns()
    {
        // if(currWaveEnemyIndex. != 0) currWaveEnemyIndex.Clear();
        // if(currSpawnsPerEnemyWave.Count != 0) currSpawnsPerEnemyWave.Clear();
        // if(currentWaveSpawns.Count != 0) currentWaveSpawns.Clear();
        // int[] currWaveEnemyIndex;
        // int[] currSpawnsPerEnemyWave; //TODO: two arrays, just set these global, reference
        switch(currentWave)
        {
            case 0: 
                currWaveEnemyIndex = waveEnemyIndex1; 
                currSpawnsPerEnemyWave = spawnsPerEnemyWave1;
                break;
            case 1: 
                currWaveEnemyIndex = waveEnemyIndex2; 
                currSpawnsPerEnemyWave = spawnsPerEnemyWave2;
                break;
            case 2: 
                currWaveEnemyIndex = waveEnemyIndex3; 
                currSpawnsPerEnemyWave = spawnsPerEnemyWave3;
                break;
            case 3: 
                currWaveEnemyIndex = waveEnemyIndex4; 
                currSpawnsPerEnemyWave = spawnsPerEnemyWave4;
                break;
            case 4: 
                currWaveEnemyIndex = waveEnemyIndex5; 
                currSpawnsPerEnemyWave = spawnsPerEnemyWave5;
                break;
            default: 
                currWaveEnemyIndex = waveEnemyIndex1;
                currSpawnsPerEnemyWave = spawnsPerEnemyWave1;
                break;
        }
    }

    Transform GetRandSpawnPosition()
    {
        // spawnRotator = commanderTransform; //set center position to commander
        float randRotation = Random.Range(0, 360);

        spawnPoint.position = new Vector3(commanderTransform.position.x + spawnDistance, commanderTransform.position.y, 0);
        // spawnPoint.RotateAround(rotatePoint, new Vector3(0, 0, 1), randRotation);
        spawnPoint.RotateAround(commanderTransform.position, Vector3.forward, randRotation);
        
        return spawnPoint;
    }

    void UpdateDisplayText()
    {
        if(stageWaveDisplay == null) return;
        stageWaveDisplay.text = "Stage " + currentStage + "/" + maxStage +"<br>" + "Wave " + (currentWave+1) + "/5";
    }

    void ToggleDisplayText(bool toggleAnyDisplay, bool finalStage = false)
    {
        //Display "Next Stage in 5s!" or "Victory in 5s!"
        if(toggleAnyDisplay)
        {
            upcomingStageDisplay.SetActive(!finalStage);
            finalStageDisplay.SetActive(finalStage);
        }else
        {
            upcomingStageDisplay.SetActive(false);
            finalStageDisplay.SetActive(false);
        }
    }

    void ToggleTimerText(bool timerToggle)
    {
        stageTimerObj.SetActive(timerToggle);
    }
}
