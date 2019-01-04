using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public List<GameObject> Platforms;

    [Space(10.0f)]
    public Transform SpawnPoint;
    public GameObject PreviousPlatform;

    private List<GameObject> _spawnedPlatforms = new List<GameObject>();

    private void Start()
    {
        _spawnedPlatforms.Add(PreviousPlatform);

        SpawnInitialPlatforms();
    }

    private void SpawnInitialPlatforms()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnNextPlatform();
        }
    }

    public void SpawnNextPlatform()
    {
        int randomNumber = Random.Range(0, Platforms.Count);

        GameObject platform = Instantiate(Platforms[randomNumber], SpawnPoint.position, Quaternion.identity);

        _spawnedPlatforms.Add(platform);

        SpawnPoint = platform.GetComponent<Platform>().SpawnTransform;
    }

    public void DestroyPreviousPlatform()
    {
        Destroy(PreviousPlatform);

        _spawnedPlatforms.Remove(PreviousPlatform);

        PreviousPlatform = _spawnedPlatforms[0];
    }
}
