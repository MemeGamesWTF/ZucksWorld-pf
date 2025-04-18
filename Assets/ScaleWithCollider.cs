using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI
using System.Runtime.InteropServices;
public class ScaleWithCollider : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private int score = 0;        // Player's score
    private int lives = 3;       // Player's lives
    private bool isGameOver = false;

    [Header("UI Elements")]
    public Text scoreText;        // Text UI for score
    public GameObject[] lifeImages; // Array for life images (e.g., 3 heart images)
    public GameObject gameOverPanel; // Game Over Panel
    public GameObject startGamePanel; // Start Game Panel
    public bool gameStart = false;
    private CameraShake cameraShake;

    [Header("Audio Settings")]
    public AudioSource audioSource; // AudioSource component for playing sound
    public AudioClip point; // AudioSource component for playing sound
    public AudioClip loss; // AudioSource component for playing sound
    public AudioClip gameover; // AudioSource component for playing sound


    [DllImport("__Internal")]
  private static extern void SendScore(int score, int game);

    void Start()
    {
        gameStart = false;
        Time.timeScale = 0f;
        // Get the BoxCollider2D and SpriteRenderer components
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogError("No CameraShake script found on the Main Camera!");
        }
        if (boxCollider == null)
        {
            Debug.LogError("No BoxCollider2D attached to the GameObject!");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer attached to the GameObject!");
        }

        // Initialize UI
        UpdateScoreUI();
        UpdateLivesUI();
        ShowStartPanel();
    }

    void Update()
    {
        if (boxCollider != null && spriteRenderer != null)
        {
            // Synchronize BoxCollider2D size with the sprite's scale
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.size = new Vector2(
                boxCollider.size.x * transform.localScale.x,
                boxCollider.size.y * transform.localScale.y
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGameOver)
            return;

        if (collision.gameObject.CompareTag("Point"))
        {
            // Earn a point
            score++;
            Debug.Log("Score: " + score);
            audioSource.PlayOneShot(point);
            // Update UI
            UpdateScoreUI();

            // Destroy the point object
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obs"))
        {
            // Lose a life
            lives--;
            Debug.Log("Lives Remaining: " + lives);
            audioSource.PlayOneShot(loss);
            

            // Update UI
            UpdateLivesUI();
            Destroy(collision.gameObject);
            if (lives <= 0)
            {
                GameOver();
            }
            else
            {
                // Trigger Camera Shake
                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(1.0f, 0.2f)); // 1 second shake with 0.2 magnitude
                }
            }
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    private void UpdateLivesUI()
    {
        // Hide life images based on remaining lives
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].SetActive(i < lives);
        }
    }

    private void GameOver()
    {
        audioSource.PlayOneShot(gameover);
        StartCoroutine(stopAllAction());
        isGameOver = true;
        Debug.Log("Game Over! Final Score: " + score);
        SendScore(score, 49);
        gameOverPanel.SetActive(true); // Show Game Over Panel
        Time.timeScale = 0f;
    }
    IEnumerator stopAllAction()
    {
        audioSource.PlayOneShot(gameover);
        yield return new WaitForSeconds(2);
        StopAllCoroutines();
    }

    public void RestartGame()
    {
        // Reset game state
        score = 0;
        lives = 3;
        isGameOver = false;
        Time.timeScale = 1f;
        // Update UI
        UpdateScoreUI();
        UpdateLivesUI();
        gameObject.GetComponent<Animator>().SetBool("scale", false);
        // Hide Game Over Panel
        gameOverPanel.SetActive(false);

        Debug.Log("Game Restarted");
        // Optionally reload the scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        gameStart = true;
        // Hide Start Game Panel and start the game
        startGamePanel.SetActive(false);
        Debug.Log("Game Started");
        Time.timeScale = 1f;
    }

    private void ShowStartPanel()
    {
        // Display the start game panel at the beginning
        startGamePanel.SetActive(true);
        
    }
}
