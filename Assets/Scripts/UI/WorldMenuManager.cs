using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnScoreRecordUpdate -= RecordUpdate;
        gameManager.OnScoreRecordUpdate += RecordUpdate;
        RecordUpdate();
    }

    private void RecordUpdate() => recordText.text = $"Рекорд\n{gameManager.GameData.BestScore}";
}
