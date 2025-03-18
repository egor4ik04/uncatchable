using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CatController))]
public class CatRandomController : MonoBehaviour
{
    [SerializeField] private float minVelocity;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float minXAddPos;
    [SerializeField] private float maxXAddPos;
    [SerializeField] private float minCD;
    [SerializeField] private float maxCD;
    [SerializeField] private float minMoveDuration;
    [SerializeField] private float maxMoveDuration;
    private float startX;
    private CatController controller;
    private GameManager gameManager;
    public bool IsRandomized;
    private bool isStarted;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnGameStart -= GameStart;
        gameManager.OnGameStart += GameStart;
        controller = GetComponent<CatController>();
        startX = transform.position.x;
    }

    private void Update()
    {
        if (!isStarted)
            RandomMoveStart();
    }

    public void RandomMoveStart()
    {
        if (IsRandomized && !isStarted)
        {
            isStarted = true;
            controller.Move(0);
            StartCoroutine(RandomMove());            
        }
    }

    private void GameStart() => isStarted = false;

    private IEnumerator RandomMove()
    {
        float moving = Random.Range(minVelocity, maxVelocity);
        int sign;
        if (transform.position.x <= startX + minXAddPos) sign = 1;
        else if (transform.position.x >= startX + maxXAddPos) sign = -1;
        else sign = Random.value < 0.5f ? 1 : -1;
        moving *= sign;
        float duration = Random.Range(minMoveDuration, maxMoveDuration);
        float currDuration = 0;
        while (!(transform.position.x <= startX + minXAddPos && moving < 0) &&
            !(transform.position.x >= startX + maxXAddPos && moving > 0) &&
            moving != 0 && currDuration < duration)
        {
            if (!IsRandomized) break;
            currDuration += Time.deltaTime;
            controller.Move(moving);
            yield return null;
        }
        controller.Move(0);
        yield return new WaitForSeconds(Random.Range(minCD, maxCD));
        if (IsRandomized)
            StartCoroutine(RandomMove());
    }
}
