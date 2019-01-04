using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int CoinScore;
    public int DistanceScore;
    public int TotalScore;

    public void AddToCoinScore(int value)
    {
        CoinScore += value;
    }
}
