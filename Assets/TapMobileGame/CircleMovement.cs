using UnityEngine;
public class CircleMovement : MonoBehaviour
{
    public Vector2 spawnArea = new Vector2(10, 10); // Area within which the circle will spawn

    private Vector3 startPosition;
    private float timeBetweenSpawns = 10.0f;

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;

        // Start invoking the method to spawn the circle every 5 seconds
        InvokeRepeating("SpawnRandomCircle", 0.0f, timeBetweenSpawns);
    }

    void SpawnRandomCircle()
    {
        float randomX = Random.Range(startPosition.x - spawnArea.x / 2, startPosition.x + spawnArea.x / 2);
        float randomY = Random.Range(startPosition.y - spawnArea.y / 2, startPosition.y + spawnArea.y / 2);
        transform.position = new Vector3(randomX, randomY, startPosition.z);
    }
}



