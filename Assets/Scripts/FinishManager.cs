using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using StarterAssets;
public class FinishManager : MonoBehaviour
{
    private static FinishManager instance;
    public static FinishManager Instance
    {
        get { return instance; }
    }
    private void Start()
    {
        instance = this;
    }
    public void NextLevel()
    {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel >= SceneManager.sceneCountInBuildSettings)
        {
            nextLevel = SceneManager.GetActiveScene().buildIndex;
        }
        SceneManager.LoadScene(nextLevel);
        InputManager.Instance.SetCursorState(true);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        InputManager.Instance.SetCursorState(true);
    }

    private void OnRestart(InputValue value)
    {
        RestartLevel();
    }
}
