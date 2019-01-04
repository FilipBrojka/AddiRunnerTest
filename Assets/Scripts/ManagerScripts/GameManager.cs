using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public PlatformManager ManagerPlatform;

    private void Start()
    {
        ManagerPlatform = GetComponent<PlatformManager>();
    }
}
