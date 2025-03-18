using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private List<GameObject> Food;
    [SerializeField] private List<GameObject> Gold;
    [SerializeField] private List<GameObject> Danger;
    [Space(5)]
    [Header("Params")]
    [SerializeField, Range(0, 1)] private float foodFrequency = 0.2f;
    public float FoodFrequency
    {
        get => foodFrequency * difficultyManager.FoodFrequencyModifier.Value;
        set => foodFrequency = Mathf.Clamp01(value);
    }
    [SerializeField, Range(0, 1)] private float goldFrequency = 0.5f;
    public float GoldFrequency
    {
        get => goldFrequency * difficultyManager.GoldFrequencyModifier.Value;
        set => goldFrequency = Mathf.Clamp01(value);
    }
    [SerializeField, Range(0, 1)] private float dangersFrequency = 0.8f;
    public float DangersFrequency
    {
        get => dangersFrequency * difficultyManager.DangersFrequencyModifier.Value;
        set => dangersFrequency = Mathf.Clamp01(value);
    }
    [SerializeField] private float minSpawnXPos = -5f;
    [SerializeField] private float maxSpawnXPos = 5f;
    [SerializeField, Min(0)] private float minSpawnCD = 0.4f;
    [SerializeField, Min(0)] private float maxSpawnCD = 3f;
    public float MinSpawnCD 
    { 
        get => minSpawnCD * difficultyManager.MinSpawnCDModifier.Value;
        set => minSpawnCD = Mathf.Max(0, value);
    }
    public float MaxSpawnCD 
    { 
        get => maxSpawnCD * difficultyManager.MaxSpawnCDModifier.Value;
        set => maxSpawnCD = Mathf.Max(0, value);
    }
    [SerializeField] public float deleteUnderAddY { get; private set; } = -2f;
    [SerializeField] private float startForceRandMin = -100f;
    [SerializeField] private float startForceRandMax = 50f;
    public float StartForceRandMin 
    {
        get => startForceRandMin * difficultyManager.StartForceRandMinModifier.Value;
        set => startForceRandMin = value; 
    }
    public float StartForceRandMax 
    {
        get => startForceRandMax * difficultyManager.StartForceRandMaxModifier.Value;
        set => startForceRandMax = value;
    }
    [SerializeField] private float startForce = 800f;
    [field:SerializeField] public bool IsSpawning { get; set; }
    
    private GameManager gameManager;
    private DifficultyManager difficultyManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        difficultyManager = FindAnyObjectByType<DifficultyManager>();
        gameManager.OnGameStart -= GameStarted;
        gameManager.OnGameStart += GameStarted;
        gameManager.OnGameEnd -= GameFinished;
        gameManager.OnGameEnd += GameFinished;
        StartCoroutine(Spawning());
    }

    private void GameStarted() => StartCoroutine(WaitingForGameStart());

    private void GameFinished() => IsSpawning = false;

    private IEnumerator WaitingForGameStart()
    {
        yield return new WaitForSeconds(gameManager.LaunchWaitTime * 2);
        IsSpawning = true;
    }

    private IEnumerator Spawning()
    {
        if (IsSpawning)
        {
            float allFreq = FoodFrequency + GoldFrequency + DangersFrequency;
            if (allFreq != 0)
            {
                float rand = Random.Range(0, allFreq);
                GameObject toSpawn;
                if (rand < FoodFrequency)
                    toSpawn = Food[Random.Range(0, Food.Count)];
                else if (rand < FoodFrequency + GoldFrequency)
                    toSpawn = Gold[Random.Range(0, Gold.Count)];
                else 
                    toSpawn = Danger[Random.Range(0, Danger.Count)];
                float addForce = Random.Range(StartForceRandMin, StartForceRandMax);
                Instantiate(toSpawn,
                    new Vector3(transform.position.x 
                    + Random.Range(minSpawnXPos, maxSpawnXPos), 0),
                    Quaternion.Euler(0, 0, Random.Range(0f, 360f)),
                    transform)
                    .GetComponent<Rigidbody2D>()
                    .AddForce(new Vector3(0, startForce + addForce));
            }            
        }
        yield return new WaitForSeconds(Random.Range(MinSpawnCD, MaxSpawnCD));
        StartCoroutine(Spawning());
    }
}
