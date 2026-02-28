using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int currentLevel = 1;
    public int playerLives = 3;
    public bool gameOver = false;
    public bool isPaused = false;
    
    public int enemiesAlive = 0;
    public GameObject exitPrefab;
    private bool exitSpawned = false;
    
    public Text livesText;
    public Text levelText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    
    private UIManager uiManager;
    
    void Awake() 
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
    
    void Start() 
    {
        uiManager = UIManager.instance;
        UpdateUI();
        enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
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
            // 重新生成玩家
            RespawnPlayer();
        }
    }
    
    void RespawnPlayer()
    {
        // 找到玩家出生点
        GameObject spawnPoint = GameObject.FindWithTag("PlayerSpawn");
        if (spawnPoint != null)
        {
            // 重新生成玩家
            GameObject playerPrefab = Resources.Load<GameObject>("Player");
            if (playerPrefab != null)
            {
                Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
            }
        }
    }
    
    public void GameOver() 
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void LevelComplete() 
    {
        currentLevel++;
        // 加载下一关
        LoadLevel(currentLevel);
    }
    
    public void LoadLevel(int level) 
    {
        // 这里可以根据关卡号加载不同的场景
        SceneManager.LoadScene("Level" + level);
    }
    
    public void EnemyKilled()
    {
        enemiesAlive--;
        
        if (enemiesAlive <= 0 && !exitSpawned)
        {
            SpawnExit();
        }
    }
    
    void SpawnExit()
    {
        // 找到出口生成点
        GameObject exitSpawn = GameObject.FindWithTag("ExitSpawn");
        if (exitSpawn != null)
        {
            Instantiate(exitPrefab, exitSpawn.transform.position, Quaternion.identity);
            exitSpawned = true;
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }
    
    public void RestartGame()
    {
        currentLevel = 1;
        playerLives = 3;
        gameOver = false;
        Time.timeScale = 1;
        LoadLevel(currentLevel);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + playerLives;
        }
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
        
        // 调用UIManager更新UI
        if (uiManager != null)
        {
            // 这里可以从玩家对象获取炸弹数量、威力和速度
            // 暂时使用默认值
            uiManager.UpdateUI(playerLives, currentLevel, 1, 1, 5f);
        }
    }
}