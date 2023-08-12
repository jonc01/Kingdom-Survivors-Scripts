using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject fullScreenCheckBox;

    void Start()
    {
        // Screen.fullScreen = false;
        // fullScreenCheckBox.SetActive(Screen.fullScreen);
    }

    void OnEnable()
    {
        // GameManager.Instance.gamePaused = true;
        // Time.timeScale = 0;
        GameManager.Instance.PauseGame();
    }

    void OnDisable()
    {
        // GameManager.Instance.gamePaused = false;
        // Time.timeScale = 1;
        GameManager.Instance.ResumeGame();
    }

    public void ToggleFullScreen()
    {
        // Screen.fullScreen = !Screen.fullScreen;
        // fullScreenCheckBox.SetActive(Screen.fullScreen);
    }
}
