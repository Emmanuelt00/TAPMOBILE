using System.Collections;
using UnityEngine;

public class Red_Alert_ScreenBlinking : MonoBehaviour
{
    public ScoreManager scoreManager; // Reference to the ScoreManager script
    public Color redAlertColor = Color.red;
    public Color coldStreakColor = Color.blue;
    private Color originalColor;

    private bool isBlinking = false;
    private float lastTapTime;
    private int consecutiveMissedTaps = 0; // Track consecutive missed taps
    private bool isInColdStreak = false;

    public Transform tapAreaCenter; // The center of the tap area (you can assign an empty GameObject as the center)
    public float tapAreaRadius = 1f; // The radius of the tap area

    void Start()
    {
        // Find and store the ScoreManager script
        scoreManager = FindObjectOfType<ScoreManager>();

        // Check if the ScoreManager script is found
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager script not found in the scene.");
        }

        // Store the original camera background color
        originalColor = Camera.main.backgroundColor;

        // Start the blinking coroutine
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // Check if the score is negative
            if (scoreManager != null && scoreManager.GetScore() < 0)
            {
                // Start the screen blink coroutine if not already blinking
                if (!isBlinking)
                {
                    StartCoroutine(BlinkScreen(redAlertColor));
                }
            }

            // Check for cold streak
            CheckColdStreak();

            // Wait for the next check
            yield return new WaitForSeconds(0.01f); // Check every 1 second
        }
    }

    void CheckColdStreak()
    {
        // Check if the player is on a cold streak
        if (!isBlinking && Time.time - lastTapTime > 5f) // Adjust the time threshold as needed
        {
            // Check if tapAreaCenter is assigned
            if (tapAreaCenter != null)
            {
                // Check if the player tapped inside the circle
                Vector2 tapPosition;
#if UNITY_EDITOR
                tapPosition = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
            tapPosition = Input.GetTouch(0).position;
#else
            tapPosition = Vector2.zero;
#endif

                if (Vector2.Distance(tapPosition, tapAreaCenter.position) > tapAreaRadius)
                {
                    // Player missed tapping inside the circle
                    consecutiveMissedTaps++;

                    if (consecutiveMissedTaps >= 10)
                    {
                        // Enter cold streak
                        StartCoroutine(BlinkScreen(coldStreakColor));
                        isInColdStreak = true;
                    }
                }
                else
                {
                    // Player tapped inside the circle, reset the consecutive missed taps
                    consecutiveMissedTaps = 0;
                    isInColdStreak = false;
                }
            }
            else
            {
                Debug.LogError("tapAreaCenter is not assigned in the Unity Editor.");
            }
        }
    }

    IEnumerator BlinkScreen(Color color)
    {
        isBlinking = true;

        // Blink the screen for 2 seconds
        Camera.main.backgroundColor = color;
        yield return new WaitForSeconds(1.50f);

        // Restore the original color after blinking
        Camera.main.backgroundColor = originalColor;

        // Wait for 3 seconds before the next blink (adjust as needed)
        yield return new WaitForSeconds(3.25f);

        isBlinking = false;
    }

    // Called when the player taps
    void OnTap()
    {
        lastTapTime = Time.time;
        // Perform other actions when the player taps
    }
}