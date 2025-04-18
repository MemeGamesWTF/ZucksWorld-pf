using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float minX = -2f; // Minimum x position
    public float maxX = 2f;  // Maximum x position

    public GameObject scalableSprite; // Reference to the sprite to scale
    public Vector2 normalSize = new Vector2(1f, 1f); // Original size of the sprite
    public Vector2 enlargedSize = new Vector2(1f, 2f); // Enlarged size along the y-axis
    public float resizeSpeed = 5f; // Speed of scaling

    private bool isScalingUp = false; // Flag to track if the sprite is being scaled up
    public Animator animator;

    [Header("Prefab Settings")]
    public GameObject[] prefabs; // Array of prefabs to spawn

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Array of spawn points

    [Header("Target Points")]
    public Transform[] targetPoints; // Array of target points corresponding to spawn points

    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed at which the object moves

    [Header("Spawn Timing")]
    public float spawnInterval = 1f; // Interval in seconds between spawns

    public ScaleWithCollider withCollider;

    [Header("Audio Settings")]
    public AudioSource audioSource; // AudioSource component for playing sound

    private void Start()
    {
        // Start the spawning process
        InvokeRepeating("SpawnRandomPrefab", 0f, spawnInterval);
    }
    void Update()
    {
        // Handle player movement
        //HandleMovement();
        if (withCollider.gameStart)
        {
            // Handle sprite scaling and sound
            if (Input.GetMouseButtonDown(0)) // Mouse button pressed
            {
                animator.SetBool("scale", true);

                // Play the sound if it's not already playing
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else if (Input.GetMouseButtonUp(0)) // Mouse button released
            {
                animator.SetBool("scale", false);

                // Stop the sound
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }


    }


    private void SpawnRandomPrefab()
    {
        // Randomly choose a prefab
        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

        // Randomly choose a spawn point
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawnPoint = spawnPoints[spawnIndex];

        // Instantiate the prefab at the chosen spawn point
        GameObject spawnedObject = Instantiate(prefabToSpawn, chosenSpawnPoint.position, Quaternion.identity);

        // Check if the spawn point is the second one (index 1)
        if (spawnIndex == 1)
        {
            // Flip the object horizontally by inverting the x scale
            Vector3 flippedScale = spawnedObject.transform.localScale;
            flippedScale.x *= -1;
            spawnedObject.transform.localScale = flippedScale;
        }

        // Move the object to the corresponding target point
        Transform targetPoint = targetPoints[spawnIndex];
        MoveObject(spawnedObject, targetPoint);
    }


    private void MoveObject(GameObject obj, Transform targetPoint)
    {
        StartCoroutine(MoveAndDestroy(obj, targetPoint));
    }

    private System.Collections.IEnumerator MoveAndDestroy(GameObject obj, Transform targetPoint)
    {
        while (obj != null && Vector3.Distance(obj.transform.position, targetPoint.position) > 0.1f)
        {
            // Move the object towards the target point
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Destroy the object once it reaches the target point
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
