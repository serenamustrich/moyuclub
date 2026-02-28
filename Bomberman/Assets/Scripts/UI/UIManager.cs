using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public Text livesText;
    public Text levelText;
    public Text bombsText;
    public Text powerText;
    public Text speedText;
    
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject winPanel;
    
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
    
    public void UpdateUI(int lives, int level, int bombs, int power, float speed)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
        if (bombsText != null)
        {
            bombsText.text = "Bombs: " + bombs;
        }
        if (powerText != null)
        {
            powerText.text = "Power: " + power;
        }
        if (speedText != null)
        {
            speedText.text = "Speed: " + speed.ToString("F1");
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void ShowPause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
    
    public void HidePause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
    
    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        GameManager.instance.RestartGame();
    }
    
    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
    
    public void ResumeGame()
    {
        GameManager.instance.TogglePause();
    }
}