using UnityEngine;

public class CircleBehavior : MonoBehaviour
{
    public GameObject circle; // Reference to the circle GameObject
    public float moveArea = 5f;  // The area within which the circle can move
    public float moveInterval = 5f;  // Time interval for moving the circle
    public float colorChangeSpeed = 0.5f; // Speed at which the color changes
    public Vector2 spawnArea = new Vector2(10, 10); // Area within which the circle will spawn
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the initial position of the circle
        startPosition = transform.position;

        // Initialize the timer
        timer = moveInterval;

        // Start by staying in the initial position
        targetPosition = startPosition;
    }

    private void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Set a new random target position
            SetNewRandomTarget();

            // Reset the timer
            timer = moveInterval;
        }

        // Move the circle towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveArea);
        Color rainbowColor = Color.HSVToRGB(Mathf.PingPong(Time.time * colorChangeSpeed, 1f), 1f, 1f);
        spriteRenderer.color = rainbowColor;
    }

    private void SetNewRandomTarget()
    {
        float randomX = Random.Range(-moveArea, moveArea);
        float randomY = Random.Range(-moveArea, moveArea);
        targetPosition = startPosition + new Vector3(randomX, randomY, 0f);
    }
}
