using System.Collections;
using UnityEngine;

public class CircleBehavior : MonoBehaviour
{
    public ScoreManager scoreManager; // Reference to the ScoreManager script
    public float moveArea = 5f;
    public float moveInterval = 5f;
    public float colorChangeSpeed = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private Collider2D circleCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        timer = moveInterval;
        targetPosition = startPosition;

        // Attempt to get the Collider2D component
        circleCollider = GetComponent<Collider2D>();

        // Check if the Collider2D component is not found
        if (circleCollider == null)
        {
            Debug.LogError("Collider2D component not found on the object.");
        }

        // Find and store the ScoreManager script
        scoreManager = FindObjectOfType<ScoreManager>();

        // Check if the ScoreManager script is found
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager script not found in the scene.");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SetNewRandomTarget();
            timer = moveInterval;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveArea);
        Color rainbowColor = Color.HSVToRGB(Mathf.PingPong(Time.time * colorChangeSpeed, 1f), 1f, 1f);
        spriteRenderer.color = rainbowColor;

        if (Input.GetMouseButtonDown(0))
        {
            CheckMouseClick();
        }
    }

    void SetNewRandomTarget()
    {
        float randomX = Random.Range(-moveArea, moveArea);
        float randomY = Random.Range(-moveArea, moveArea);
        targetPosition = startPosition + new Vector3(randomX, randomY, 0f);
    }

    void CheckMouseClick()
    {
        if (circleCollider != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            if (circleCollider.bounds.Contains(mousePosition))
            {
                // Increment the score by 1 when clicking inside the circle
                scoreManager.IncrementScore();
            }
            else
            {
                // Decrement the score by 1 when clicking outside the circle
                scoreManager.DecrementScore();
            }
        }
        else
        {
            Debug.LogError("Collider2D component is null.");
        }
    }
}

