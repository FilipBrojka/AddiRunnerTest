using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int Value = 1;

    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = GameManager.instance.Score;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _scoreManager.AddToCoinScore(Value);
            Destroy(gameObject);
        }
    }
}
