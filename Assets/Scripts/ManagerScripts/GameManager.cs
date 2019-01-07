using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameStateType { Playing, EndGame, Idle }
    public GameStateType GameState;

    [Space(10.0f)]
    public PlatformManager ManagerPlatform;
    public ScoreManager Score;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        ManagerPlatform = GetComponent<PlatformManager>();
        Score = GetComponent<ScoreManager>();
    }

    private void Start()
    {
        GameState = GameStateType.Idle;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
