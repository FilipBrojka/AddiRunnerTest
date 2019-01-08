using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{    
    private GameManager _gameManager;
    private ScoreManager _scoreManager;
    private AddiController _addiController;

    private void Start()
    {
        _gameManager = GameManager.instance;
        _scoreManager = _gameManager.Score;
        _addiController = AddiController.instance;        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            _gameManager.GameState = GameManager.GameStateType.EndGame;

            _scoreManager.CompareNewScoreAndSaveHighScores(_scoreManager.TotalScore);
            _scoreManager.ShowEndGameCanvas();

            _addiController.AddiAC.SetTrigger("HitObstacle");

            print("Player Died!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _gameManager.GameState = GameManager.GameStateType.EndGame;

            _scoreManager.CompareNewScoreAndSaveHighScores(_scoreManager.TotalScore);
            _scoreManager.ShowEndGameCanvas();

            _addiController.AddiAC.SetTrigger("Fall");

            print("Player Died!");
        }
    }
}
