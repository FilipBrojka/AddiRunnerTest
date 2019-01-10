using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public AudioSource ObstacleHitAudioSource;

    [Space(10.0f)]
    public AudioClip ObstacleHitClip;
    public AudioClip ObstacleJumpedOnClip;
    public AudioClip FallThroughHoleClip;

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

            if(_addiController.AddiAC.GetBool("Glide") || _addiController.AddiAC.GetBool("Falling"))
            {
                ObstacleHitAudioSource.clip = ObstacleJumpedOnClip;
                ObstacleHitAudioSource.Play();
            }

            if(_addiController.AddiAC.GetBool("Run"))
            {
                ObstacleHitAudioSource.clip = ObstacleHitClip;
                ObstacleHitAudioSource.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CompareTag("Hole") && collision.CompareTag("Player"))
        {
            _gameManager.GameState = GameManager.GameStateType.EndGame;

            _scoreManager.CompareNewScoreAndSaveHighScores(_scoreManager.TotalScore);
            _scoreManager.ShowEndGameCanvas();

            _addiController.AddiAC.SetTrigger("Fall");

            ObstacleHitAudioSource.clip = FallThroughHoleClip;
            ObstacleHitAudioSource.Play();
        }
    }
}
