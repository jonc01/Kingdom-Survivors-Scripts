using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy_Spawner : MonoBehaviour
{
    //
    [SerializeField] public bool startSpawning;
    [SerializeField] TextMeshProUGUI stageWaveDisplay;
    [Header("Spawn Locations")]
    [SerializeField] Transform commanderTransform;
    [SerializeField] Transform spawnPoint;
    private Commander_Combat commanderCombat;
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
    [Header("*Needs Setup")]
    [SerializeField] GameObject[] enemies;
    [SerializeField] float spawnDistance = 3;

    //Spawners 
    [Space(10)]
    [Header("= Spawns - Auto setup each Wave =")]
    [SerializeField] int[] curr_numSpawnsInWave; //How many enemies of [idx] to spawn
    [SerializeField] float[] curr_spawnFrequencyTimer; //0.5f
    [Header("= Spawns Debug =")]
    [SerializeField] private int[] curr_totalSpawnsPerEnemy; //Total spawns per enemy per wave
    [SerializeField] private float[] curr_spawnTimer; //enemy specific spawn timers
    [SerializeField] private float[] curr_spawnCounterPerEnemy; //Count num spawns per enemy
    [SerializeField] private bool[] enemySpawnToggle; //when Toggled, run timers to spawn

    [Space(10)]
    [Header("= Waves Setup =")]
    [SerializeField] int maxStage = 5;
    [Header("= Waves Debug (Managed at runtime) =")]
    [SerializeField] private int currentStage;
    [SerializeField] private int currentWave;
    [SerializeField] public bool stagesCleared;
    private float stageTimer;
    [SerializeField] private int[] totalEnemySpawnsPerWave; //total different enemy types spawned each wave

    [Space(20)]
    [Header("MAIN SETUP")]
    [Header("-(1)- Waves: NumSpawnsPerEnemy (Needs Setup) (Override format) -")]
    [Header("- Override parameters with total number of different Enemies -")]
    //Functions like an overload function, of each enemy in order (0, 1, 2, 3, 4, 5)
    [SerializeField] int[] numEnemySpawnsPerWave1; //Ex: { 20, 5 } //spawns 20 of Enemy0, 5 of Enemy1
    [SerializeField] int[] numEnemySpawnsPerWave2; // { 0, 5, 10 } //Spawns 5 of Enemy1, etc
    [SerializeField] int[] numEnemySpawnsPerWave3; // { 0, 0, 20, 10 }
    [SerializeField] int[] numEnemySpawnsPerWave4; // {  }
    [SerializeField] int[] numEnemySpawnsPerWave5; // { 30, 20, 0, 10, 0, 1 }
    [Space(10)]
    [Header("-(2)- Waves: SpawnFrequencyPerEnemy (Needs Setup) (Override format) -")]
    [SerializeField] float[] spawnFrequencyPerEnemyWave1;
    [SerializeField] float[] spawnFrequencyPerEnemyWave2;
    [SerializeField] float[] spawnFrequencyPerEnemyWave3;
    [SerializeField] float[] spawnFrequencyPerEnemyWave4;
    [SerializeField] float[] spawnFrequencyPerEnemyWave5;

    [Space(20)]
    [Header("-(3)- Bonus Waves -")]
    [SerializeField] private int bonus_totalEnemySpawnsPerWave;
    [SerializeField] int[] bonus_numEnemySpawnsPerWave;
    [SerializeField] float[] bonus_spawnFrequencyPerEnemyWave;
    [SerializeField] private int bonus_totalSpawns;
    

    [Space(20)]
    [Header("= (!-Gets set automatically) =")]
    [SerializeField] private EnemyUpgrade enemyUpgrade;
    

    void Start()
    {
        DisableEnemySpawns();
        ResetTimers();
        ResetCounters();
        InitWaveCounter();
        InitBonusWaveCounter();

        stageTimer = 0;
        commanderTransform = GameManager.Instance.PlayerTransform;
        commanderCombat = commanderTransform.GetComponent<Commander_Combat>();
        enemyLevel = 1;

        stagesCleared = false;
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
        UpdateTimers();

        //Stage 
        if(stageTimer <= 0)
        {
            ToggleDisplayText(false);
            ToggleTimerText(false);

            if(stagesCleared)
            {
                // GameManager.Instance.GameOver(); //TODO: TESTING remove
                return;
            }
            return;
        }else{
            ToggleTimerText(true);
        }
        stageTimer -= Time.deltaTime;
        stageTimerDisplay.text = stageTimer.ToString("N0") + "s";
    }
    
    void UpdateTimers()
    {
        if(enemySpawnToggle[0]){
            curr_spawnTimer[0] -= Time.deltaTime;
            if(curr_spawnTimer[0] <= 0) SpawnEnemy(0);
        }
        if(enemySpawnToggle[1]){
            curr_spawnTimer[1] -= Time.deltaTime; 
            if(curr_spawnTimer[1] <= 0) SpawnEnemy(1); 
        }
        if(enemySpawnToggle[2]){
            curr_spawnTimer[2] -= Time.deltaTime; 
            if(curr_spawnTimer[2] <= 0) SpawnEnemy(2);
        }
        if(enemySpawnToggle[3]){
            curr_spawnTimer[3] -= Time.deltaTime;
            if(curr_spawnTimer[3] <= 0) SpawnEnemy(3);
        }
        if(enemySpawnToggle[4]){
            curr_spawnTimer[4] -= Time.deltaTime;
            if(curr_spawnTimer[4] <= 0) SpawnEnemy(4);
        }
        if(enemySpawnToggle[5]){
            curr_spawnTimer[5] -= Time.deltaTime;
            if(curr_spawnTimer[5] <= 0) SpawnEnemy(5);
        }
    }


    void SpawnEnemy(int enemyIndex)
    {
        // if(stagesCleared) return; //Stop spawning once all Stages are cleared
        if(stagesCleared)
        {
            //If stages are cleared, start next Bonus wave instead
            if(bonus_totalEnemySpawnsPerWave <= 0) StartBonusWave();
            bonus_totalEnemySpawnsPerWave--;
        }
        else if(totalEnemySpawnsPerWave[currentWave] <= 0) StartNewWave();

        //Check if spawn count has reached the spawn total
        if(ReachedSpawnTotal(enemyIndex))
        {
            ToggleEnemySpawn(enemyIndex, false);
            return;
        }

        //Increment enemy counter, then reset to spawn frequency
        curr_spawnCounterPerEnemy[enemyIndex]++;
        totalEnemySpawnsPerWave[currentWave]--;
        curr_spawnTimer[enemyIndex] = curr_spawnFrequencyTimer[enemyIndex];

        if(stagesCleared) UpdateBonusDisplayText();
        else UpdateDisplayText();

        //Spawn Enemy and set stats
        GameObject enemyToSpawn = enemies[enemyIndex];
        Enemy_Combat enemyCombat = enemyToSpawn.GetComponent<Enemy_Combat>();
        enemyCombat.level = enemyLevel;
        enemyCombat.enemyUpgrade = enemyUpgrade;
        //Get Spawn position, then spawn
        Transform randSpawnPos = GetRandSpawnPosition();
        Instantiate(enemyToSpawn, randSpawnPos.position, Quaternion.identity, transform);
    }

    private bool ReachedSpawnTotal(int enemyIdx)
    {
        //return true if spawnCounter has reached or exceeded totalSpawns
        return curr_spawnCounterPerEnemy[enemyIdx] >= curr_totalSpawnsPerEnemy[enemyIdx];
    }

    void StartNewWave()
    {
        currentWave++;

        //New Stage check
        if(currentWave > 4) //currentWave is Index
        {
            currentWave = 0;
            currentStage++;
            if(currentStage > maxStage)
            {
                stagesCleared = true;
                stageTimer = 10; //TODO: allow victory, but start spawning endless
                ToggleDisplayText(true, true); //"Victory in: "
                //TODO: display Bonus Wave notification
                Invoke("StartBonusWave", stageTimer);
                return;
            }
            enemyLevel++;
            InitWaveCounter();
        }
        GetWaveSpawns();
        ResetTimers();
        ResetCounters();
    }

    private void GetWaveSpawns()
    {
        //Waves are setup in Inspector for numEnemySpawnsPerWave1-5
        DisableEnemySpawns();
        int[] curr_numEnemySpawnsPerWave;
        float[] curr_spawnFrequencyPerWave;
        switch(currentWave)
        {
            case 0:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave1;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave1;
                break;
            case 1:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave2;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave2;
                break;
            case 2:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave3;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave3;
                break;
            case 3:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave4;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave4;
                break;
            case 4:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave5;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave5;
                break;
            default:
                curr_numEnemySpawnsPerWave = numEnemySpawnsPerWave3;
                curr_spawnFrequencyPerWave = spawnFrequencyPerEnemyWave1;
                break;
        }

        for(int i=0; i<enemies.Length; i++)
        {
            int totalSpawns = curr_numEnemySpawnsPerWave[i];
            curr_totalSpawnsPerEnemy[i] = totalSpawns;
            curr_spawnFrequencyTimer[i] = curr_spawnFrequencyPerWave[i];
            //Only Toggle Enemy if totalSpawns isn't 0
            if(totalSpawns > 0)
            {
                ToggleEnemySpawn(i);
            }
        }
    }

    void StartBonusWave()
    {
        currentStage++;
        enemyLevel++;
        
        // float curr_ScoreMult = (currentStage - maxStage) * .1f;
        GameManager.Instance.UpdateScoreMultiplier(.1f);

        UpdateBonusDisplayText();
        bonus_totalEnemySpawnsPerWave = bonus_totalSpawns;
        
        //Set Bonus wave spawns - overriding GetWaveSpawns()
        for(int i=0; i<enemies.Length; i++)
        {
            int totalSpawns = bonus_numEnemySpawnsPerWave[i];
            curr_totalSpawnsPerEnemy[i] = totalSpawns;
            curr_spawnFrequencyTimer[i] = bonus_spawnFrequencyPerEnemyWave[i];
            //Only Toggle Enemy if totalSpawns isn't 0
            if(totalSpawns > 0)
            {
                ToggleEnemySpawn(i);
            }
        }

        //Spawn 1 giant Wave per Stage
        ResetTimers();
        ResetCounters();
    }

    void InitBonusWaveCounter()
    {
        //Get total number of spawns each wave
        for(int j=0; j<bonus_numEnemySpawnsPerWave.Length; j++)
        {
            bonus_totalEnemySpawnsPerWave += bonus_numEnemySpawnsPerWave[j];
        }
        bonus_totalSpawns = bonus_totalEnemySpawnsPerWave;
    }

#region --
    private void ResetTimers()
    {
        for(int i=0; i<curr_spawnTimer.Length; i++)
        {
            curr_spawnTimer[i] = curr_spawnFrequencyTimer[i];
        }
    }

    private void ResetCounters()
    {
        for(int i=0; i<curr_spawnCounterPerEnemy.Length; i++)
        {
            curr_spawnCounterPerEnemy[i] = 0;
        }
    }

    private void ToggleEnemySpawn(int index, bool toggle = true)
    {
        enemySpawnToggle[index] = toggle;
    }

    private void DisableEnemySpawns()
    {
        for(int i=0; i<5; i++)
        {
            enemySpawnToggle[i] = false;
        }
    }

    private void InitWaveCounter()
    {
        int[] temp_numEnemySpawnsPerWave;
        int totalWaves = 5; //Temp until replacing with custom number of waves

        //Get total number of spawns each wave
        for(int tempWave=0; tempWave<totalWaves; tempWave++)
        {
            temp_numEnemySpawnsPerWave = GetNumEnemySpawns(tempWave);
            for(int j=0; j<temp_numEnemySpawnsPerWave.Length; j++)
            {
                totalEnemySpawnsPerWave[tempWave] += temp_numEnemySpawnsPerWave[j];
            }
        }
    }

    int[] GetNumEnemySpawns(int waveIdx)
    {
        switch(waveIdx)
        {
            case 0: return numEnemySpawnsPerWave1;
            case 1: return numEnemySpawnsPerWave2;
            case 2: return numEnemySpawnsPerWave3;
            case 3: return numEnemySpawnsPerWave4;
            case 4: return numEnemySpawnsPerWave5;
            default: return null;
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
#endregion

#region UI Display
    void UpdateDisplayText()
    {
        if(stageWaveDisplay == null) return;
        stageWaveDisplay.text = "Stage " + currentStage + "/" + maxStage +"<br>" + "Wave " + (currentWave+1) + "/5";
    }

    void UpdateBonusDisplayText()
    {
        if(stageWaveDisplay == null) return;
        stageWaveDisplay.text = "Bonus Stage " + (currentStage - maxStage) + "<br>" + "Enemy Lv." + enemyLevel;
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
#endregion
}
