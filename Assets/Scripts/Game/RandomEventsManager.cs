using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEventsManager : MonoBehaviour
{
    [SerializeField] private Material eventMaterial;
    [SerializeField] private IRandomEventType currentState;
    public IRandomEventType CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            OnRandomEventStart?.Invoke(currentState);
        }
    }
    [SerializeField] private float eventColorChangeDuration = 1f;
    [SerializeField, Range(0, 1)] private float baseEventChance = 0.25f;
    public float BaseEventChance
    {
        get => baseEventChance * difficultyManager.BaseEventChanceModifier.Value;
        set => baseEventChance = Mathf.Clamp01(value);
    }
    [SerializeField, Range(0, 1)] private float foodRushEventChance = 0.25f;
    public float FoodRushEventChance
    {
        get => foodRushEventChance * difficultyManager.FoodRushEventChanceModifier.Value;
        set => foodRushEventChance = Mathf.Clamp01(value);
    }
    [SerializeField, Range(0, 1)] private float goldRushEventChance = 0.5f;
    public float GoldRushEventChance
    {
        get => goldRushEventChance * difficultyManager.GoldRushEventChanceModifier.Value;
        set => goldRushEventChance = Mathf.Clamp01(value);
    }
    [SerializeField, Range(0, 1)] private float dangersRushEventChance = 0.25f;
    public float DangersRushEventChance
    {
        get => dangersRushEventChance * difficultyManager.DangersRushEventChanceModifier.Value;
        set => dangersRushEventChance = Mathf.Clamp01(value);
    }
    [field: SerializeField] public RandomEvent BaseState { get; set; }
    [SerializeField] private List<RandomEvent> randomEvents;

    private readonly int ShaderColorId = Shader.PropertyToID("_Color");
    private GameManager gameManager;
    private DifficultyManager difficultyManager;
    private ObjectSpawner spawner;

    public event Action<IRandomEventType> OnRandomEventStart;

    [Serializable]
    public class RandomEvent
    {
        public IRandomEventType EventType;
        public Color EventColor;
        public float MinSpawnCD;
        public float MaxSpawnCD;
        public float MinDuration;
        public float MaxDuration;
        public float StartForceRandMin;
        public float StartForceRandMax;
        public float FoodFrequency;
        public float GoldFrequency;
        public float DangersFrequency;
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        difficultyManager = FindAnyObjectByType<DifficultyManager>();
        spawner = FindAnyObjectByType<ObjectSpawner>();
        gameManager.OnGameStart -= resetEvent;
        gameManager.OnGameStart += resetEvent;
        gameManager.OnGameEnd -= resetEvent;
        gameManager.OnGameEnd += resetEvent;
        eventMaterial.SetColor(ShaderColorId, BaseState.EventColor);
    }

    private void resetEvent()
    {
        StopAllCoroutines();
        startRandomEvent(IRandomEventType.baseState);
        spawner.MinSpawnCD = BaseState.MinSpawnCD;
        spawner.MaxSpawnCD = BaseState.MaxSpawnCD;
        spawner.DangersFrequency = BaseState.DangersFrequency;
        spawner.FoodFrequency = BaseState.FoodFrequency;
        spawner.GoldFrequency = BaseState.GoldFrequency;
        spawner.StartForceRandMin = BaseState.StartForceRandMin;
        spawner.StartForceRandMax = BaseState.StartForceRandMax;
    }
    private void startRandomEvent(IRandomEventType? randomEvent = null)
    {
        if (randomEvent is null)
        {
            List<RandomEvent> events = new List<RandomEvent>();
            events.Add(BaseState); 
            events.AddRange(randomEvents);

            float allFreq = BaseEventChance + FoodRushEventChance + GoldRushEventChance + DangersRushEventChance;
            if (allFreq != 0)
            {
                //do {
                    float rand = Random.Range(0, allFreq);
                    if (rand < BaseEventChance)
                        randomEvent = IRandomEventType.baseState;
                    else if (rand < BaseEventChance + FoodRushEventChance)
                        randomEvent = IRandomEventType.foodRush;
                    else if (rand < BaseEventChance + FoodRushEventChance + GoldRushEventChance)
                        randomEvent = IRandomEventType.goldRush;
                    else
                        randomEvent = IRandomEventType.dangersRush;
                //} while (randomEvent == CurrentState && FoodRushEventChance + GoldRushEventChance + DangersRushEventChance > 0 && BaseEventChance > 0);
            }
            else
                randomEvent = IRandomEventType.baseState;

            //events.Remove(events.First(e => e.EventType == CurrentState));
            //randomEvent = events[Random.Range(0, events.Count)].EventType;
        }
        IRandomEventType randomEventType = randomEvent ?? IRandomEventType.baseState;
        RandomEvent start = CurrentState == IRandomEventType.baseState ? BaseState : randomEvents.SingleOrDefault(e => e.EventType == CurrentState);
        RandomEvent end = randomEventType == IRandomEventType.baseState ? BaseState : randomEvents.SingleOrDefault(e => e.EventType == randomEventType);

        StartCoroutine(eventChange(start, end));
        CurrentState = randomEventType;
    }
    private IEnumerator eventChange(RandomEvent startEvent, RandomEvent endEvent)
    {
        if (gameManager.IsGameStarted || endEvent.EventType == IRandomEventType.baseState)
        {
            StartCoroutine(switchColor(startEvent.EventColor, endEvent.EventColor, eventColorChangeDuration));
            yield return new WaitForSeconds(eventColorChangeDuration);
            StartCoroutine(switchColor(endEvent.EventColor, startEvent.EventColor, eventColorChangeDuration));
            yield return new WaitForSeconds(eventColorChangeDuration);
            StartCoroutine(switchColor(startEvent.EventColor, endEvent.EventColor, eventColorChangeDuration));
            yield return new WaitForSeconds(eventColorChangeDuration);
            StartCoroutine(switchColor(endEvent.EventColor, startEvent.EventColor, eventColorChangeDuration));
            yield return new WaitForSeconds(eventColorChangeDuration);
            StartCoroutine(switchColor(startEvent.EventColor, endEvent.EventColor, eventColorChangeDuration));
            yield return new WaitForSeconds(eventColorChangeDuration);

            spawner.MinSpawnCD = endEvent.MinSpawnCD;
            spawner.MaxSpawnCD = endEvent.MaxSpawnCD;
            spawner.DangersFrequency = endEvent.DangersFrequency;
            spawner.FoodFrequency = endEvent.FoodFrequency;
            spawner.GoldFrequency = endEvent.GoldFrequency;
            spawner.StartForceRandMin = endEvent.StartForceRandMin;
            spawner.StartForceRandMax = endEvent.StartForceRandMax;

            yield return new WaitForSeconds(Random.Range(endEvent.MinDuration, endEvent.MaxDuration));

            if (gameManager.IsGameStarted)
                startRandomEvent();
        }
    }
    private IEnumerator switchColor(Color start, Color end, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Color currentColor = Color.Lerp(start, end, t);
            timeElapsed += Time.deltaTime;

            eventMaterial.SetColor(ShaderColorId, currentColor);

            yield return null;
        }

        eventMaterial.SetColor(ShaderColorId, end);
    }
}

public enum IRandomEventType
{
    baseState, foodRush, goldRush, dangersRush
}
