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
}