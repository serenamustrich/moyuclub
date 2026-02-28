using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public Text scoreText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public bool isGameOver = false;
    public bool isGamePaused = false;
    public bool isGameStarted = false;
    public Button startButton;
    public Text startButtonText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 初始化UI
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        finalScoreText = GameObject.Find("FinalScoreText").GetComponent<Text>();
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        
        // 隐藏游戏结束面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // 初始化游戏状态
        isGameStarted = false;
        isGamePaused = false;
        Time.timeScale = 0; // 初始暂停游戏
        UpdateStartButtonText();
    }

    public void AddScore(int points)
    {
        score += points;
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        isGameOver = true;
        isGameStarted = false;
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + score;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        UpdateStartButtonText();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        if (!isGameStarted)
        {
            // 开始游戏
            isGameStarted = true;
            isGamePaused = false;
            Time.timeScale = 1;
        }
        else
        {
            // 切换暂停/继续
            isGamePaused = !isGamePaused;
            Time.timeScale = isGamePaused ? 0 : 1;
        }
        UpdateStartButtonText();
    }

    private void UpdateStartButtonText()
    {
        if (!isGameStarted)
        {
            startButtonText.text = "开始游戏";
        }
        else if (isGamePaused)
        {
            startButtonText.text = "继续游戏";
        }
        else
        {
            startButtonText.text = "暂停游戏";
        }
    }
}