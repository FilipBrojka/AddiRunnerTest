using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SaveLoadNamespace;

public class ScoreManager : MonoBehaviour
{
    public Canvas EndGameCanvas;
    public Canvas SettingsCanvas;
    public Canvas AreYouSureCanvas;

    [Space(10.0f)]
    public GameObject NewHighScoreBaloons;

    [Space(10.0f)]
    public Text HighScoreText;
    public Text PlayerScoreText;    

    [Space(10.0f)]
    public Text TotalScoreText;
    public Text FinalScoreText;

    [Space(10.0f)]
    public Text NumberOfSquatsText;
    public Text NumberOfJumpsText;
    public Text NumberOfGlidesText;

    [Space(10.0f)]
    public int TotalScore;

    [Space(10.0f)]
    public int NumberOfSquats = 0;
    public int NumberOfJumps = 0;
    public int NumberOfGlides = 0;
   
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
        if (!SettingsCanvas.enabled)
        {
            EndGameCanvas.enabled = true;
        }
    }

    public void HideEndGameCanvas()
    {
        EndGameCanvas.enabled = false;
    }

    public void ShowSettingsCanvas()
    {
        SettingsCanvas.enabled = true;
    }

    public void DisableSettingsCanvas()
    {
        SettingsCanvas.enabled = false;
    }

    public void EnableAreYouSureCanvas()
    {
        AreYouSureCanvas.enabled = true;
    }

    public void DisableAreYouSureCanvas()
    {
        AreYouSureCanvas.enabled = false;
    }

    public void CompareNewScoreAndSaveHighScores(int score)
    {
        HighScoreData loadedData = BinarySerializer.Load<HighScoreData>();

        if (loadedData == null)
        {
            HighScoreText.text = "New High Score: " + TotalScore;
            PlayerScoreText.text = "Your Score: " + TotalScore;

            NewHighScoreBaloons.SetActive(true);

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

                NewHighScoreBaloons.SetActive(true);
            }
            else
            {
                HighScoreText.text = "High Score: " + loadedData.HighScore;
                PlayerScoreText.text = "Your Score: " + TotalScore;
            }
        }

        ShowStatistics();
    }

    private void ShowStatistics()
    {
        NumberOfJumpsText.text = "Jumps executed: " + NumberOfJumps;
        NumberOfGlidesText.text = "Glides executed: " + NumberOfGlides;
        NumberOfSquatsText.text = "Squats executed: " + NumberOfSquats;
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
