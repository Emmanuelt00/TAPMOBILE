using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Reference to the UI Text element for displaying the score
    private int score = 0;

    void Start()
    {
        // Initialize the score to 0
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        // Update the UI text to display "Score: X" where X is the current score
        scoreText.text = "Score: " + score;
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
}
