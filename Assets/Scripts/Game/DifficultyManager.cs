using System;
using System.Collections;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static int DifficultySteps { get; private set; }
    public static int DifficultyCurrentStep { get; private set; }

    [SerializeField, Min(1)] private int steps;
    public int Steps
    {
        get => steps;
        set
        {
            if (value < 1) value = 1;
            steps = value;
            DifficultySteps = steps;
        }
    }
    [SerializeField, Min(1)] private int currentStep;
    public int CurrentStep
    {
        get => currentStep;
        set
        {
            if (value < 1) value = 1;
            currentStep = value;
            DifficultyCurrentStep = currentStep;
        }
    }
    [SerializeField] private float stepToStepInterval = 20f;

    [Header("Spawner")]
    public DifficultyFloatPropertyModifier FoodFrequencyModifier;
    public DifficultyFloatPropertyModifier GoldFrequencyModifier;
    public DifficultyFloatPropertyModifier DangersFrequencyModifier;
    public DifficultyFloatPropertyModifier MinSpawnCDModifier;
    public DifficultyFloatPropertyModifier MaxSpawnCDModifier;
    public DifficultyFloatPropertyModifier StartForceRandMinModifier;
    public DifficultyFloatPropertyModifier StartForceRandMaxModifier;
    [Space(5)]
    [Header("Events")]
    public DifficultyFloatPropertyModifier BaseEventChanceModifier;
    public DifficultyFloatPropertyModifier FoodRushEventChanceModifier;
    public DifficultyFloatPropertyModifier GoldRushEventChanceModifier;
    public DifficultyFloatPropertyModifier DangersRushEventChanceModifier;

    private GameManager gameManager;

    private void Awake()
    {
        Steps = steps;
        CurrentStep = currentStep;
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnGameEnd -= resetValues;
        gameManager.OnGameEnd += resetValues;
        gameManager.OnGameStart -= startDifficultyCalculation;
        gameManager.OnGameStart += startDifficultyCalculation;
        resetValues();
    }

    private void resetValues()
    {
        StopAllCoroutines();
        CurrentStep = 1; 
    }
    private void startDifficultyCalculation()
    {
        resetValues();
        StartCoroutine(changeDifficulty());
    }
    private IEnumerator changeDifficulty()
    {
        if (gameManager.IsGameStarted)
        {
            yield return new WaitForSeconds(stepToStepInterval);
            CurrentStep = Mathf.Clamp(CurrentStep + 1, 1, Steps);
            StartCoroutine(changeDifficulty());
        }
    }
}