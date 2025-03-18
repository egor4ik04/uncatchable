using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "UnCATchable/Data/Game")]
public class GameData : ScriptableObject
{
    private int bestScore;
    public int BestScore 
    {
        get => bestScore; 
        set
        {
            bestScore = value;
            OnBestScoreUpdate?.Invoke();
        }
    }

    public event Action OnBestScoreUpdate;
}
