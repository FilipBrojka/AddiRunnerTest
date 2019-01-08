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
    public Text TotalScoreText;
    public Text FinalScoreText;

    [Space(10.0f)]
    public int TotalScore;
   
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    public void AddToTotalScore(int amount)
    {
        TotalScore += amount;
        TotalScoreText.text = TotalScore.ToString();
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
