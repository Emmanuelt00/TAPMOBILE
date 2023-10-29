using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupScreenController : MonoBehaviour
{
    void Update()
    {
        // Check for mouse click or tap on a touch screen
        if (Input.GetMouseButtonDown(0))
        {
                // Load the game level scene
                SceneManager.LoadScene("LEVEL I"); // Replace "GameLevel" with the actual name of your game level scene
        }
    }
}