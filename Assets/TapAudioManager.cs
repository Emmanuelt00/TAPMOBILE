using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TappingAudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Ensure the AudioSource is not playing at the start
        audioSource.Stop();
    }

    private void Update()
    {
        // Check for mouse click or tap on a touch screen
        if (Input.GetMouseButtonDown(0))
        {
            // Play the tapping sound
            audioSource.Play();
        }
    }
}