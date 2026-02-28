using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public static TetrisGrid instance;
    public int width = 12;
    public int height = 20;
    public Transform[,] grid;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        grid = new Transform[height, width];
    }

    public bool IsValidPosition(Transform pieceTransform, Vector2Int position, Vector2Int[,] shape)
    {
        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (shape[y, x] == 1)
                {
                    int newX = position.x + x;
                    int newY = position.y + y;

                    if (newX < 0 || newX >= width || newY < 0 || newY >= height)
                    {
                        return false;
                    }

                    if (grid[newY, newX] != null)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void LockPiece(Transform pieceTransform, Vector2Int position, Vector2Int[,] shape, Material material)
    {
        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (shape[y, x] == 1)
                {
                    int newX = position.x + x;
                    int newY = position.y + y;

                    if (newY >= 0 && newY < height && newX >= 0 && newX < width)
                    {
                        // 创建方块并添加到网格
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.transform.position = new Vector3(newX, newY, 0);
                        block.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                        block.GetComponent<Renderer>().material = material;
                        block.transform.parent = transform;

                        grid[newY, newX] = block.transform;
                    }
                }
            }
        }

        // 检查是否有行可以消除
        ClearLines();
    }

    private void ClearLines()
    {
        int linesCleared = 0;

        for (int y = height - 1; y >= 0; y--)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                linesCleared++;
                y++;
            }
        }

        if (linesCleared > 0)
        {
            // 计算得分
            int[] linePoints = { 0, 100, 300, 500, 800 };
            int points = linePoints[linesCleared] * GameManager.instance.level;
            GameManager.instance.AddScore(points);
            GameManager.instance.AddLines(linesCleared);
        }
    }

    private bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[y, x] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearLine(int y)
    {
        // 销毁该行的所有方块
        for (int x = 0; x < width; x++)
        {
            if (grid[y, x] != null)
            {
                Destroy(grid[y, x].gameObject);
                grid[y, x] = null;
            }
        }

        // 将上面的行向下移动
        for (int i = y; i > 0; i--)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[i - 1, x] != null)
                {
                    grid[i, x] = grid[i - 1, x];
                    grid[i, x].position = new Vector3(x, i, 0);
                    grid[i - 1, x] = null;
                }
            }
        }
    }
}