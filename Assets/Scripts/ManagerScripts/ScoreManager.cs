using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SaveLoadNamespace;

public class ScoreManager : MonoBehaviour
{
    public Canvas EndGameCanvas;

    [Space(10.0f)]
    public Text HighScoreText;
    public Text PlayerScoreText;    

    [Space(10.0f)]
    public Text CoinScoreText;
    public Text DistanceScoreText;
    public Text TotalScoreText;

    [Space(10.0f)]
    public int CoinScore;
    public int DistanceScore;
    public int TotalScore;
   
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    public void AddToCoinScore(int amount)
    {
        CoinScore += amount;
        CoinScoreText.text = CoinScore.ToString();
    }

    public void AddToDistanceScore(int amount)
    {
        DistanceScore += amount;
        DistanceScoreText.text = DistanceScore.ToString();
    }

    public void CalculateTotalScore()
    {
        TotalScore = DistanceScore + CoinScore;

        CompareNewScoreAndSaveHighScores(TotalScore);
    }

    public void ShowEndGameCanvas()
    {
        EndGameCanvas.enabled = true;
    }

    public void HideEndGameCanvas()
    {
        EndGameCanvas.enabled = false;
    }

    public void CompareNewScoreAndSaveHighScores(int score)
    {
        HighScoreData loadedData = BinarySerializer.Load<HighScoreData>();

        if (loadedData == null)
        {
            HighScoreText.text = "New High Score: " + TotalScore;
            PlayerScoreText.text = "Your Score: " + TotalScore;

            HighScoreData highScoreData = new HighScoreData();
            BinarySerializer.Save(highScoreData);
        }
        else
        {
            if (TotalScore > loadedData.HighScore)
            {
                HighScoreData highScoreData = new HighScoreData();

                highScoreData.HighScore = TotalScore;

                BinarySerializer.Save(highScoreData);

                HighScoreText.text = "New High Score: " + TotalScore;
                PlayerScoreText.text = "Your Score: " + TotalScore;
            }
            else
            {
                HighScoreText.text = "High Score: " + loadedData.HighScore;
                PlayerScoreText.text = "Your Score: " + TotalScore;
            }
        }        
    }

    public void DeleteSavedData()
    {
        BinarySerializer.Delete<HighScoreData>();
    }

    [System.Serializable]
    public class HighScoreData
    {
        public int HighScore;
    }
}
