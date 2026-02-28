using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Text scoreText;
    public Text gameOverText;
    public Button restartButton;
    public GameObject player;
    public GameObject[] enemySpawnPoints;
    public GameObject[] enemyPrefabs;
    public float enemySpawnInterval = 3f;
    
    private int score = 0;
    private bool gameOver = false;
    private float nextEnemySpawnTime = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateScoreText();
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (!gameOver && Time.time >= nextEnemySpawnTime)
        {
            SpawnEnemy();
            nextEnemySpawnTime = Time.time + enemySpawnInterval;
        }
    }
    
    void SpawnEnemy()
    {
        int spawnPointIndex = Random.Range(0, enemySpawnPoints.Length);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        
        Instantiate(enemyPrefabs[enemyIndex], enemySpawnPoints[spawnPointIndex].transform.position, Quaternion.identity);
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }
    
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
    
    public void GameOver()
    {
        gameOver = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        // 播放游戏结束音效
        AudioManager.Instance.PlaySound("GameOver");
    }
    
    public void RestartGame()
    {
        // 重新加载场景
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
