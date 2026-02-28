using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject foodPrefab;
    public GameObject powerFoodPrefab;
    public GameObject pacmanPrefab;
    public GameObject ghostPrefab;

    private char[,] map = {
        { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
        { '#', '.', '#', '#', '#', '#', '#', '#', '#', '#', '.', '#' },
        { '#', '.', '#', ' ', ' ', ' ', ' ', ' ', ' ', '#', '.', '#' },
        { '#', '.', '#', ' ', '#', '#', '#', '#', ' ', '#', '.', '#' },
        { '#', '.', '.', ' ', '#', 'o', '#', 'o', ' ', '.', '.', '#' },
        { '#', '#', '#', ' ', '#', '#', '#', '#', ' ', '#', '#', '#' },
        { ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
        { '#', '#', '#', ' ', '#', '#', '#', '#', ' ', '#', '#', '#' },
        { '#', '.', '.', ' ', ' ', ' ', ' ', ' ', ' ', '.', '.', '#' },
        { '#', '.', '#', ' ', '#', '#', '#', '#', ' ', '#', '.', '#' },
        { '#', '.', '#', ' ', ' ', ' ', ' ', ' ', ' ', '#', '.', '#' },
        { '#', '.', '#', '#', '#', '#', '#', '#', '#', '#', '.', '#' },
        { '#', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
    };

    private void Start()
    {
        // 尝试加载预制体
        LoadPrefabs();
        GenerateLevel();
    }

    private void LoadPrefabs()
    {
        // 在实际项目中，这里应该从Resources文件夹加载预制体
        // 这里为了演示，我们假设预制体已经在场景中设置
    }

    private void GenerateLevel()
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3(x, rows - y - 1, 0);

                switch (map[y, x])
                {
                    case '#':
                        if (wallPrefab != null)
                            Instantiate(wallPrefab, position, Quaternion.identity);
                        break;
                    case '.':
                        if (foodPrefab != null)
                            Instantiate(foodPrefab, position, Quaternion.identity);
                        break;
                    case 'o':
                        if (powerFoodPrefab != null)
                            Instantiate(powerFoodPrefab, position, Quaternion.identity);
                        break;
                }
            }
        }

        // 放置吃豆豆和幽灵
        if (pacmanPrefab != null)
            Instantiate(pacmanPrefab, new Vector3(6, 1, 0), Quaternion.identity);
        if (ghostPrefab != null)
        {
            Instantiate(ghostPrefab, new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(ghostPrefab, new Vector3(6, 7, 0), Quaternion.identity);
            Instantiate(ghostPrefab, new Vector3(7, 7, 0), Quaternion.identity);
        }
    }
}