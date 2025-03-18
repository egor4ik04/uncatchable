using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float LaunchWaitTime = 3f;
    [field: SerializeField] public int MaxLifes { get; private set; } = 9;
    [SerializeField] private int lifes;
    public int Lifes 
    {
        get => lifes;
        set {
            lifes = value;
            if (lifes > MaxLifes)
                lifes = MaxLifes;
            if (lifes <= 0)
            {
                lifes = 0;
                EndGame();
            }
            OnHealthChange?.Invoke();
        } 
    }
    [SerializeField] private int score;
    public int Score
    { 
        get => score;
        set
        {
            score = value;
            if (score < 0) 
                score = 0;
            OnScoreChange?.Invoke();
        }
    }
    [SerializeField] public GameData GameData;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Canvas GameUI;
    [SerializeField] private Canvas WorldUI;
    private GameObject Cat;
    public bool IsGameStarted;
    private bool canBeFinished;

    public event Action OnHealthChange;
    public event Action OnScoreChange;
    public event Action OnScoreRecordUpdate;
    public event Action OnGameStart;
    public event Action OnGameEnd;

    private void Start()
    {
        Cat = FindAnyObjectByType<CatController>().gameObject;
        GameData.OnBestScoreUpdate -= OnScoreRecordUpdate;
        GameData.OnBestScoreUpdate += OnScoreRecordUpdate;
    }

    public void StartGame()
    {
        if (!IsGameStarted)
        {
            IsGameStarted = true;
            Lifes = MaxLifes;
            Score = 0;
            StartCoroutine(GameLaunching());
            OnGameStart?.Invoke();
        }
    }

    private void EndGame()
    {
        if (IsGameStarted && canBeFinished)
        {
            IsGameStarted = false;
            StartCoroutine(GameLaunching());
            GameData.BestScore = Mathf.Max(GameData.BestScore, Score);
            OnGameEnd?.Invoke();
        }
    }

    private IEnumerator GameLaunching()
    {
        if (IsGameStarted) canBeFinished = false;
        cameraTarget.position = new Vector3 (0, 50, 0);
        Cat.GetComponent<CatController>().IsControllable = false;
        Cat.GetComponent<CatRandomController>().IsRandomized = false;
        yield return new WaitForSeconds(LaunchWaitTime/2);        
        GameUI.gameObject.SetActive(IsGameStarted);
        WorldUI.gameObject.SetActive(!IsGameStarted);
        yield return new WaitForSeconds(LaunchWaitTime/2);
        OnHealthChange?.Invoke();
        if (IsGameStarted)
        {
            Cat.GetComponent<CatController>().IsControllable = true;
            Cat.transform.position = new Vector3(8, 9, 0);
            cameraTarget.position = new Vector3(8, 9f, 0);
        }
        else {
            Cat.GetComponent<CatRandomController>().IsRandomized = true;
            Cat.transform.position = new Vector3(6.5f, -2.59f, 0);
            cameraTarget.position = new Vector3(0, 0, 0);
        }
        canBeFinished = IsGameStarted;
    }
}
