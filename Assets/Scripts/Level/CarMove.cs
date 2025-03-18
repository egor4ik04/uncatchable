using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarMove : MonoBehaviour
{
    [SerializeField] private Vector2 minCorner;
    [SerializeField] private Vector2 maxCorner;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 1.5f;
    [SerializeField] private float minDelay = 0.5f;
    [SerializeField] private float maxDelay = 5f;

    [Space(10)]
    [Header("Look Not Touch")]
    [SerializeField] private bool isStarting;
    [SerializeField] private float speed;
    [SerializeField] private float destinationX;
    [SerializeField] private float destinationY;

    void Start()
    {
        StartCoroutine(StartDrive());
    }

    void Update()
    {
        if (!isStarting)
        {
            if (Mathf.Abs(transform.localPosition.x - destinationX) < 0.1f)
            {
                StartCoroutine(StartDrive());
            }
            else
            {
                transform.localPosition = Vector2.MoveTowards(
                    transform.localPosition,
                    new Vector2(destinationX, destinationY),
                    speed * Time.deltaTime);
            }
        }
    }

    private IEnumerator StartDrive()
    {
        isStarting = true;
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        bool goLeft = Random.value < 0.5f;
        GetComponent<SpriteRenderer>().flipX = goLeft;
        destinationY = Random.Range(minCorner.y, maxCorner.y);
        speed = Random.Range(minSpeed, maxSpeed);
        if (!goLeft)
        {
            destinationX = maxCorner.x;
            transform.localPosition = new Vector2(minCorner.x, destinationY);
        }
        else
        {
            destinationX = minCorner.x;
            transform.localPosition = new Vector2(maxCorner.x, destinationY);
        }
        yield return null;
        isStarting = false;
    }
}
