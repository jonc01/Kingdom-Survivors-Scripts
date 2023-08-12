using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LootLocker.Requests;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    Scene currScene;
    [Space(20)]
    [SerializeField] bool leaderboardToggle = true;
    [Space(20)]
    [SerializeField] public GameObject enemySpawner;
    [SerializeField] Enemy_Spawner enemy_Spawner;
    [SerializeField] GameObject StartScreenObj;
    [SerializeField] GameObject PauseMenuObj;
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] private GameObject leaderboardParentObj;
    public TMP_InputField playerNameInputField;
    [SerializeField] GameObject scoreSubmitButton;
    [SerializeField] TextMeshProUGUI submittedName;
    [SerializeField] private float totalScore;
    [SerializeField] public float scoreMultiplier = 1; //Default 1, mult
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] TextMeshProUGUI stageWaveDisplay;

    [SerializeField] public Transform PlayerTransform;
    [SerializeField] Commander_Combat commanderCombat;
    [SerializeField] DamageNumbersPoolManager damageNumbersPoolManager;
    [SerializeField] DamageNumbersPoolManager healingNumbersPoolManager;
    [Header("Health")]
    [SerializeField] public PlayerHealth playerHealth;

    [Header("Gold/XP/Score")]
    [SerializeField] Transform XPOrbParentObj;
    // [SerializeField] GameObject[] GoldXPPrefab;
    [SerializeField] ObjectPoolManager[] XPPools;
    [SerializeField] GameObject HealPrefab;
    [SerializeField] TextMeshProUGUI scoreMultiplierDisplay;
    [SerializeField] TextMeshProUGUI goldDisplay;
    [SerializeField] int totalGold;
    [Space(10)]
    [Header("Start Game - Ref")]
    [SerializeField] GameObject[] toggleElements;
    [SerializeField] GameObject[] toggleElements2;
    [SerializeField] GameObject[] toggleSubmitElements;
    [Space(10)]
    [Header("Game Over - Score Screen")]
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI finalStageWave;
    [SerializeField] GameObject GameOverScreenParent;
    [SerializeField] GameObject DefeatScreen;
    [SerializeField] GameObject VictoryScreen;
    public bool gamePaused;
    public bool shopOpen;
    public bool gameIsOver;
    private bool gameStarted;
    
    void Awake()
    {
        Instance = this;
        if(PlayerTransform == null) PlayerTransform = GameObject.FindGameObjectWithTag("Commander").transform;
        commanderCombat = PlayerTransform.GetComponent<Commander_Combat>();
        gamePaused = false;
        if(damageNumbersPoolManager == null) damageNumbersPoolManager = DamageNumbersPoolManager.Instance;
        if(enemySpawner == null) enemySpawner = GameObject.FindGameObjectWithTag("Spawner").gameObject;
        if(enemy_Spawner == null) enemy_Spawner = enemySpawner.GetComponent<Enemy_Spawner>();

        shopOpen = false;
        currScene = SceneManager.GetActiveScene();
        ToggleGameUI(false, false);
    }

    void Start()
    {
        gameIsOver = false;
        // Time.timeScale = 1.0f;
        // Time.timeScale = 0f;

        gameStarted = false;
        StartGamePrompt(true);
        totalScore = 0;
        totalGold = 0;
        AddScore(0); //sets up
        AddGold(0);
        scoreMultiplier = 1;
        UpdateScoreMultiplier(0);

        #if !UNITY_EDITOR
            leaderboardToggle = true;
        #endif
        if(leaderboardToggle)
            StartCoroutine(LLSetupRoutine());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(gameStarted) return;
            // gameStarted = true;
            StartGamePrompt(false);
            StartCoroutine(StartGameTransition());
        }
        if(!gameStarted) return;
        if(shopOpen) return;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // StartGamePrompt(false);
            TogglePauseMenu();
        }


        if(gamePaused) return;
        //
    }

    IEnumerator StartGameTransition()
    {
        yield return new WaitForSeconds(.1f);
        ToggleGameUI(true, true);
        StartGamePrompt(false);
    }

    public void StartGamePrompt(bool toggle)
    {
        if(gameStarted) return;
        StartScreenObj.SetActive(toggle);
        gamePaused = toggle;
        if(!toggle) gameStarted = true;
        else Time.timeScale = 0f;
        // gamePaused = toggle;
        // if(toggle) Time.timeScale = 0f;
        // else Time.timeScale = 1f;
    }

    void ToggleGameUI(bool toggleUI, bool togglePlayerHeroes)
    {
        for(int i=0; i<toggleElements.Length; i++)
        {
            toggleElements[i].SetActive(toggleUI);
        }
    
        for(int i=0; i<toggleElements2.Length; i++)
        {
            toggleElements2[i].SetActive(togglePlayerHeroes);
        }

    }

    void ToggleSubmitUI(bool toggle)
    {
        for(int i=0; i<toggleSubmitElements.Length; i++)
        {
            toggleSubmitElements[i].SetActive(toggle);
        }
    }

    void TogglePauseMenu()
    {
        bool currentToggle = PauseMenuObj.activeInHierarchy;
        PauseMenuObj.SetActive(!currentToggle);
    }

    public void AddScore(float score)
    {
        if(gameIsOver) return;
        totalScore += (score * scoreMultiplier);
        scoreDisplay.text = totalScore.ToString("N0");
        // commanderCombat.AddXP(score); //Not adding XP on kills
    }

    public void SpawnXP(Vector3 spawnPos, int xpTier = 0)
    {
        // Instantiate(GoldXPPrefab[xpTier], spawnPos, Quaternion.identity, XPOrbParentObj);

        GameObject xpOrb = XPPools[xpTier].GetPooledObject();
        xpOrb.transform.position = spawnPos;
        xpOrb.SetActive(true);

        // int rand = Random.Range(0, 101);
        // if(rand >= 100) SpawnHeal(spawnPos);
        if(Random.value < .01f) SpawnHeal(spawnPos);
    }

    public void SpawnHeal(Vector3 spawnPos)
    {
        Instantiate(HealPrefab, spawnPos, Quaternion.identity, transform);
    }

    public void AddGold(int addedGold)
    {
        totalGold += addedGold;
        goldDisplay.text = totalGold.ToString("N0");
    }

    public void UpdateScoreMultiplier(float multiplier)
    {
        scoreMultiplier += multiplier;
        if(scoreMultiplierDisplay != null)
            scoreMultiplierDisplay.text = "x" + scoreMultiplier.ToString("N1");
    }

    public void GameOver()
    {
        Time.timeScale = 0.5f;
        gameIsOver = true;
        StartCoroutine(StartGameOver());
    }

    IEnumerator StartGameOver()
    {
        ToggleGameUI(false, true);
        yield return new WaitForSecondsRealtime(.5f);
        Time.timeScale = .1f;
        yield return new WaitForSecondsRealtime(1f);
        // PauseGame();
        finalStageWave.text = stageWaveDisplay.text;
        finalScore.text = totalScore.ToString("N0");

        //Display Game Over screen
        GameOverScreenParent.SetActive(true);
        Time.timeScale = 0;

        //Display Victory or Defeat screen, submit score if Victory
        // if(commanderCombat.isAlive) VictoryScreen.SetActive(true);
        if(enemy_Spawner.stagesCleared) VictoryScreen.SetActive(true);
        else
        {
            DefeatScreen.SetActive(true);
            StartCoroutine(GetLeaderboardOnly());
        } 
        // yield return leaderboard.SubmitScoreCO(totalScore); //moved to Victory screen toggle
    }

    public void PauseGame()
    {
        if(gameIsOver) return;
        Time.timeScale = 0;
        gamePaused = true;
    }

    public void ResumeGame()
    {
        if(gameIsOver) return;
        gamePaused = false;
        GameOverScreenParent.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        if(currScene == null) currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.name);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }

    public void SpawnDamageNumber(float damage, Vector3 position)
    {
        DamageNumbersHandler damageNum = damageNumbersPoolManager.GetPooledObject(damage, position);
        damageNum.gameObject.SetActive(true);
    }

    public void SpawnHealNumber(float healAmount, Vector3 position)
    {
        DamageNumbersHandler healNum = healingNumbersPoolManager.GetPooledObject(healAmount, position);
        healNum.gameObject.SetActive(true);
    }

#region LootLocker Leaderboard

    public void SetPlayerName()
    {
        playerNameInputField.enabled = true;
        LootLockerSDKManager.SetPlayerName(playerNameInputField.text, (response) =>
        {
            if(response.success)
            {
                Debug.Log("Successfully set Player name");
                StartCoroutine(SubmitScore());
            }
            else
            {
                Debug.Log("Could not set Player name"+response.Error);
            }
        });
    }

    IEnumerator SubmitScore()
    {
        //
        Debug.Log("Submit Score first");

        int scoreToSubmit = (int)totalScore;

        yield return leaderboard.SubmitScoreCO(scoreToSubmit);
        playerNameInputField.enabled = false;
        ToggleSubmitUI(false);
        submittedName.text = playerNameInputField.text;
        yield return leaderboard.FetchTopHighScoresCO();
        Debug.Log("Display leaderboard");
    }

    IEnumerator GetLeaderboardOnly()
    {
        //Update Leaderboard, don't take a score from the Player
        yield return leaderboard.FetchTopHighScoresCO();
        Debug.Log("Display leaderboard");
    }

    IEnumerator LLSetupRoutine()
    {
        yield return LoginRoutine();
        yield return leaderboard.FetchTopHighScoresCO();
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if(response.success)
            {
                Debug.Log("Successful Login");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
            }
        });
        yield return new WaitWhile(()=>done == false);
    }
#endregion
}
