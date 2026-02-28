using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score = 0;
    public int level = 1;
    public int lines = 0;
    public float dropInterval = 1.0f;
    public bool isGameOver = false;
    public bool isGamePaused = false;
    
    public Text scoreText;
    public Text levelText;
    public Text linesText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
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
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        linesText = GameObject.Find("LinesText").GetComponent<Text>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        finalScoreText = GameObject.Find("FinalScoreText").GetComponent<Text>();
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButtonText = GameObject.Find("StartButtonText").GetComponent<Text>();
        
        // 隐藏游戏结束面板
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // 初始化UI文本
        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void AddLines(int count)
    {
        lines += count;
        
        // 更新等级
        int newLevel = Mathf.FloorToInt(lines / 10) + 1;
        if (newLevel > level)
        {
            level = newLevel;
            dropInterval = Mathf.Max(0.1f, 1.0f - (level - 1) * 0.1f);
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        if (levelText != null)
            levelText.text = "Level: " + level;
        if (linesText != null)
            linesText.text = "Lines: " + lines;
    }

    public void GameOver()
    {
        isGameOver = true;
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + score;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ToggleGame()
    {
        if (!isGameOver)
        {
            isGamePaused = !isGamePaused;
            UpdateStartButtonText();
        }
    }

    public void StartGame()
    {
        isGamePaused = false;
        UpdateStartButtonText();
    }

    private void UpdateStartButtonText()
    {
        if (startButtonText != null)
        {
            if (isGameOver)
            {
                startButtonText.text = "重新开始";
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
}