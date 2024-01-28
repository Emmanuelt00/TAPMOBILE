using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Reference to the UI Text element for displaying the score
    public Text messageText; // Reference to the UI Text element for displaying messages
    private int score = 0;
    private int maxScore = 50; // The maximum score to reach
    private int minScore = -1; // The minimum score to lose
    private bool gameEnded = false;

    void Start()
    {
        // Initialize the score to 0
        UpdateScoreText();
        // Initialize the message text
        messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check if the game has already ended
        if (gameEnded)
            return;

        // Check for mouse click or tap on a touch screen
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the maximum score is reached
            if (score >= maxScore)
            {
                // Add your win logic here
                Debug.Log("Congratulations! You won!");
                EndGame("YOU, WON ! LOADING NEXT LEVEL... ");
                // Exit the game after 5 seconds
                StartCoroutine(ExitGameAfterDelay(5.0f));
            }
            // Check if the minimum score is reached
            else if (score <= minScore)
            {
                // Add your loss logic here
                Debug.Log("Sorry, you loss !");
                EndGame("YOU, LOSE ! TRY AGAIN. ");
                // Restart the level after 2 seconds
                StartCoroutine(RestartLevelAfterDelay(2.0f));
            }

            // Update the UI text to display "SCORE = X" where X is the current score
            UpdateScoreText();
        }
    }

    void UpdateScoreText()
    {
        // Update the UI text to display "SCORE = X" where X is the current score
        scoreText.text = "SCORE = " + score.ToString();
        // Change the text color to yellow
        scoreText.color = Color.black;

        // Set the font style to bold and change the font size
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.fontSize = 80; // Adjust the font size as needed
    }

    void EndGame(string endMessage)
    {
        // Set the gameEnded flag to true to prevent further updates
        gameEnded = true;

        // Set the message text
        messageText.text = endMessage;
        // Enable the message text
        messageText.gameObject.SetActive(true);
    }

    public void IncrementScore()
    {
        // Increment the score by 1
        score++;

        // Update the UI text to display the current score
        UpdateScoreText();
    }

    public void DecrementScore()
    {
        // Decrement the score by 1
        score--;

        // Update the UI text to display the current score
        UpdateScoreText();
    }

    IEnumerator ExitGameAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

#if UNITY_STANDALONE
        // Only call Application.Quit() in standalone builds
        Application.Quit();
#else
    // In the Unity Editor, stop play mode
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator RestartLevelAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetScore()
    {
        return score;
    }
}

