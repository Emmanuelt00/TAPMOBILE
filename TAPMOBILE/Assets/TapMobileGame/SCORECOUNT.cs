using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Reference to the UI Text element for displaying the score
    public GameObject circle; // Reference to the circle GameObject
    private int score = 0;
    private int maxScore = 100; // The maximum score to reach

    void Start()
    {
        // Initialize the score to 0
        UpdateScoreText();
    }

    void Update()
    {
        // Check for mouse click or tap on a touch screen
        if (Input.GetMouseButtonDown(0) && score < maxScore)
        {
            Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(tapPosition);

            if (hitCollider != null)
            {
                if (hitCollider.gameObject == circle)
                {
                    // Tapped inside the circle
                    score++;
                }
                else
                {
                    // Tapped outside the circle
                    score--;
                }
            }

            if (hitCollider != null)
            {
                if (hitCollider.gameObject == circle)
                {
                    // Tapped inside the circle
                    score++;
                    Debug.Log("Tapped inside the circle. Score: " + score);
                }
                else
                {
                    // Tapped outside the circle
                    score--;
                    Debug.Log("Tapped outside the circle. Score: " + score);
                }
            }


            // Update the UI text to display the current score
            UpdateScoreText();

            // Check if the maximum score is reached
            if (score >= maxScore)
            {
                // Add your exit logic here (e.g., Application.Quit())
                Debug.Log("Score reached 100. Exiting the program.");
                Application.Quit();
            }
        }
    }

    void UpdateScoreText()
    {
        // Update the UI text to display "SCORE = X" where X is the current score
        scoreText.text = "SCORE = " + score.ToString();
        // Change the text color to yellow
        scoreText.color = Color.yellow;

        // Set the font style to bold and change the font size
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.fontSize = 80; // Adjust the font size as needed
    }
}