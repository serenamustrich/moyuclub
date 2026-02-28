using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject[,] mapGrid = new GameObject[13, 13];
    public GameObject wallPrefab;
    public GameObject steelPrefab;
    public GameObject waterPrefab;
    public GameObject forestPrefab;
    public GameObject basePrefab;
    public GameObject floorPrefab;
    public Transform mapParent;

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
        LoadLevel(1);
    }

    public void LoadLevel(int level)
    {
        // 清除现有地图
        ClearMap();
        
        // 根据关卡加载不同的地图
        switch (level)
        {
            case 1:
                LoadLevel1();
                break;
            case 2:
                LoadLevel2();
                break;
            // 可以继续添加其他关卡
            default:
                LoadLevel1();
                break;
        }
    }

    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void LoadLevel1()
    {
        // 13x13网格地图
        // 0: 地板, 1: 砖墙, 2: 钢板, 3: 海水, 4: 森林, 5: 基地
        int[,] level1Map = new int[13, 13] {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 1, 2, 1, 0, 1, 2, 1, 0, 1, 2, 1, 0},
            {0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
            {0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0}
        };

        GenerateMap(level1Map);
    }

    private void LoadLevel2()
    {
        // 第二关地图
        int[,] level2Map = new int[13, 13] {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0},
            {0, 1, 2, 0, 1, 2, 2, 2, 1, 0, 2, 1, 0},
            {0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1},
            {1, 2, 1, 0, 1, 2, 5, 2, 1, 0, 1, 2, 1},
            {1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0},
            {0, 1, 2, 0, 1, 2, 2, 2, 1, 0, 2, 1, 0},
            {0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };

        GenerateMap(level2Map);
    }

    private void GenerateMap(int[,] mapData)
    {
        float cellSize = 1.0f;
        float startX = -6.0f;
        float startY = 6.0f;

        for (int y = 0; y < 13; y++)
        {
            for (int x = 0; x < 13; x++)
            {
                Vector3 position = new Vector3(startX + x * cellSize, startY - y * cellSize, 0);
                GameObject prefab = null;

                switch (mapData[y, x])
                {
                    case 1:
                        prefab = wallPrefab;
                        break;
                    case 2:
                        prefab = steelPrefab;
                        break;
                    case 3:
                        prefab = waterPrefab;
                        break;
                    case 4:
                        prefab = forestPrefab;
                        break;
                    case 5:
                        prefab = basePrefab;
                        break;
                    default:
                        prefab = floorPrefab;
                        break;
                }

                if (prefab != null)
                {
                    mapGrid[x, y] = Instantiate(prefab, position, Quaternion.identity, mapParent);
                }
            }
        }
    }
}
