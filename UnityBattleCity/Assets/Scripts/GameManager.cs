using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerLives = 3;
    public int score = 0;
    public Text livesText;
    public Text scoreText;
    public Text gameOverText;
    public GameObject gameOverPanel;
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public float respawnDelay = 2f;
    public int currentLevel = 1;
    public Text levelText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
        gameOverPanel.SetActive(false);
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
    }

    public void PlayerDied()
    {
        playerLives--;
        UpdateUI();
        
        if (playerLives <= 0)
        {
            GameOver();
        }
        else
        {
            Invoke("SpawnPlayer", respawnDelay);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel <= 35)
        {
            UpdateUI();
            // 加载下一关
            LevelManager.instance.LoadLevel(currentLevel);
        }
        else
        {
            // 游戏胜利
            GameWin();
        }
    }

    private void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + playerLives;
        }
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!\nFinal Score: " + score;
        }
    }

    private void GameWin()
    {
        gameOverPanel.SetActive(true);
        if (gameOverText != null)
        {
            gameOverText.text = "You Win!\nFinal Score: " + score;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        // 这里可以加载菜单场景，暂时返回首页
        Application.OpenURL("/");
    }
}
