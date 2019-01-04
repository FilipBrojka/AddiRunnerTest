using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    private PlatformManager _platformManager;

    private void Start()
    {
        _platformManager = GameManager.instance.ManagerPlatform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _platformManager.SpawnNextPlatform();
            _platformManager.DestroyPreviousPlatform();
        }
    }
}
