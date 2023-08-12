using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameCheck : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Instance.PauseGame();
    }

    void OnDisable()
    {
        GameManager.Instance.ResumeGame();
    }
}
