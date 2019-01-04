using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text CoinScoreText;
    public Text DistanceScoreText;

    [Space(10.0f)]
    public int CoinScore;
    public int DistanceScore;
    public int TotalScore;

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
}
