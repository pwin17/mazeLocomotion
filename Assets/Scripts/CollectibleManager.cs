using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance;

    public GameObject collectiblePrefab; // Score collectible
    public GameObject timeCollectiblePrefab; // Time collectible
    public Transform mazeParent;
    public int numberOfCollectibles = 5;
    public int numberOfTimeCollectibles = 5;
    public Vector3 minPosition;
    public Vector3 maxPosition;

    public TextMeshProUGUI scoreAndTimeDisplay;
    public TextMeshProUGUI gameOverDisplay;

    private int score;
    private float timeRemaining = 600f; // Set the initial playtime in seconds (5 minutes = 300 seconds)
    private bool isGameOver = false;
    public AudioSource soundEffect;
    public AudioSource soundEffect_1;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SpawnCollectibles();
        gameOverDisplay.gameObject.SetActive(false);
        soundEffect = GetComponent<AudioSource>();
        soundEffect_1 = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Update the timer
            timeRemaining -= Time.deltaTime;

            // Check if the time has run out
            if (timeRemaining <= 0)
            {
                GameOver();
            }

            UpdateScoreAndTimeUI();
        }
    }

    public void IncrementScore()
    {
        score++;
        Debug.Log("Current Score" + score);
        soundEffect.Play();
        UpdateScoreAndTimeUI();
    }

    public void AddTime(float addedTime)
    {
        timeRemaining += addedTime;
        Debug.Log("Current Time" + timeRemaining);
        soundEffect_1.Play();
        UpdateScoreAndTimeUI();
    }

 
    
    private void SpawnCollectibles()
    {
        int spawnedScoreCollectibles = 0;
        int spawnedTimeCollectibles = 0;

        while (spawnedScoreCollectibles < numberOfCollectibles || spawnedTimeCollectibles < numberOfTimeCollectibles)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y),
                Random.Range(minPosition.z, maxPosition.z)
            );

            // Check if the position is free of obstacles
            Collider[] colliders = Physics.OverlapSphere(randomPosition, 0.5f);
            bool isFree = true;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Obstacle"))
                {
                    isFree = false;
                    break;
                }
            }

            if (isFree)
            {
                if (spawnedScoreCollectibles < numberOfCollectibles)
                {
                    Instantiate(collectiblePrefab, randomPosition, Quaternion.identity, mazeParent);
                    spawnedScoreCollectibles++;
                }
                else if (spawnedTimeCollectibles < numberOfTimeCollectibles)
                {
                    Instantiate(timeCollectiblePrefab, randomPosition, Quaternion.identity, mazeParent);
                    spawnedTimeCollectibles++;
                }
            }
        }
    }


    private void UpdateScoreAndTimeUI()
    {
        scoreAndTimeDisplay.text = $"Score: {score}\nTime: {Mathf.Max(0, Mathf.FloorToInt(timeRemaining))}s";
    }

    private void GameOver()
    {
        isGameOver = true;
        timeRemaining = 0f;
        gameOverDisplay.gameObject.SetActive(true);
        gameOverDisplay.text = $"Game Over - > Total Score: {score}";
        // Disable player controls, end the game or trigger another event here.
    }



}
