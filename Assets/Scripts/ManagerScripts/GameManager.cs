using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
}
