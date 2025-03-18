using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private int coinsByItem;
    [SerializeField] private int healthByItem;
    [SerializeField] private float sideForce = 0f;
    [SerializeField] private float minRotationSpeed = -1f;
    [SerializeField] private float maxRotationSpeed = 1f;
    private GameManager gameManager;
    private ObjectSpawner spawner;
    private float rotation;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        spawner = GetComponentInParent<ObjectSpawner>();
        rotation = Random.Range(minRotationSpeed, maxRotationSpeed);
        GetComponent<Rigidbody2D>().AddForce(
            new Vector2(Random.Range(-sideForce, sideForce), 0));
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 
            rotation + transform.eulerAngles.z);
        if (transform.position.y < spawner.transform.position.y + spawner.deleteUnderAddY
            || !gameManager.IsGameStarted)
            DeleteItem();
    }

    private void DeleteItem() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.OnGameEnd -= DeleteItem;
            gameManager.Lifes += healthByItem;
            gameManager.Score += coinsByItem;
            DeleteItem();
        }
    }
}
