using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    public enum Tetromino
    {
        I, J, L, O, S, T, Z
    }

    public Tetromino type;
    public Vector2Int position;
    public Vector2Int[,] shape;
    public Material material;

    private float dropTimer = 0f;

    private void Start()
    {
        SpawnRandomPiece();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused)
            return;

        // 处理输入
        HandleInput();

        // 自动下落
        dropTimer += Time.deltaTime;
        if (dropTimer >= GameManager.instance.dropInterval)
        {
            MoveDown();
            dropTimer = 0f;
        }
    }

    private void HandleInput()
    {
        // 左右移动
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        // 加速下落
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }

        // 旋转
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Rotate();
        }

        // 硬降
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    public void SpawnRandomPiece()
    {
        // 随机生成方块类型
        Tetromino[] types = (Tetromino[])System.Enum.GetValues(typeof(Tetromino));
        type = types[Random.Range(0, types.Length)];

        // 设置方块形状
        shape = GetShape(type);

        // 设置初始位置
        position = new Vector2Int(5, 0);

        // 设置颜色
        material = GetMaterial(type);

        // 绘制方块
        DrawPiece();

        // 检查游戏是否结束
        if (!TetrisGrid.instance.IsValidPosition(transform, position, shape))
        {
            GameManager.instance.GameOver();
        }
    }

    private Vector2Int[,] GetShape(Tetromino type)
    {
        switch (type)
        {
            case Tetromino.I:
                return new Vector2Int[,] {
                    {0, 0, 0, 0},
                    {1, 1, 1, 1},
                    {0, 0, 0, 0},
                    {0, 0, 0, 0}
                };
            case Tetromino.J:
                return new Vector2Int[,] {
                    {1, 0, 0},
                    {1, 1, 1},
                    {0, 0, 0}
                };
            case Tetromino.L:
                return new Vector2Int[,] {
                    {0, 0, 1},
                    {1, 1, 1},
                    {0, 0, 0}
                };
            case Tetromino.O:
                return new Vector2Int[,] {
                    {1, 1},
                    {1, 1}
                };
            case Tetromino.S:
                return new Vector2Int[,] {
                    {0, 1, 1},
                    {1, 1, 0},
                    {0, 0, 0}
                };
            case Tetromino.T:
                return new Vector2Int[,] {
                    {0, 1, 0},
                    {1, 1, 1},
                    {0, 0, 0}
                };
            case Tetromino.Z:
                return new Vector2Int[,] {
                    {1, 1, 0},
                    {0, 1, 1},
                    {0, 0, 0}
                };
            default:
                return new Vector2Int[,] {
                    {1, 1},
                    {1, 1}
                };
        }
    }

    private Material GetMaterial(Tetromino type)
    {
        Material mat = new Material(Shader.Find("Standard"));
        switch (type)
        {
            case Tetromino.I:
                mat.color = Color.cyan;
                break;
            case Tetromino.J:
                mat.color = Color.blue;
                break;
            case Tetromino.L:
                mat.color = Color.orange;
                break;
            case Tetromino.O:
                mat.color = Color.yellow;
                break;
            case Tetromino.S:
                mat.color = Color.green;
                break;
            case Tetromino.T:
                mat.color = Color.magenta;
                break;
            case Tetromino.Z:
                mat.color = Color.red;
                break;
            default:
                mat.color = Color.white;
                break;
        }
        return mat;
    }

    private void DrawPiece()
    {
        // 清除现有方块
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 绘制新方块
        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (shape[y, x] == 1)
                {
                    int newX = position.x + x;
                    int newY = position.y + y;

                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.transform.position = new Vector3(newX, newY, 0);
                    block.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                    block.GetComponent<Renderer>().material = material;
                    block.transform.parent = transform;
                }
            }
        }
    }

    public void MoveLeft()
    {
        Vector2Int newPosition = position + new Vector2Int(-1, 0);
        if (TetrisGrid.instance.IsValidPosition(transform, newPosition, shape))
        {
            position = newPosition;
            DrawPiece();
        }
    }

    public void MoveRight()
    {
        Vector2Int newPosition = position + new Vector2Int(1, 0);
        if (TetrisGrid.instance.IsValidPosition(transform, newPosition, shape))
        {
            position = newPosition;
            DrawPiece();
        }
    }

    public void MoveDown()
    {
        Vector2Int newPosition = position + new Vector2Int(0, -1);
        if (TetrisGrid.instance.IsValidPosition(transform, newPosition, shape))
        {
            position = newPosition;
            DrawPiece();
        }
        else
        {
            // 锁定方块
            TetrisGrid.instance.LockPiece(transform, position, shape, material);
            // 生成新方块
            SpawnRandomPiece();
        }
    }

    public void HardDrop()
    {
        while (true)
        {
            Vector2Int newPosition = position + new Vector2Int(0, -1);
            if (TetrisGrid.instance.IsValidPosition(transform, newPosition, shape))
            {
                position = newPosition;
                DrawPiece();
            }
            else
            {
                break;
            }
        }
        // 锁定方块
        TetrisGrid.instance.LockPiece(transform, position, shape, material);
        // 生成新方块
        SpawnRandomPiece();
    }

    public void Rotate()
    {
        Vector2Int[,] rotatedShape = RotateMatrix(shape);
        if (TetrisGrid.instance.IsValidPosition(transform, position, rotatedShape))
        {
            shape = rotatedShape;
            DrawPiece();
        }
    }

    private Vector2Int[,] RotateMatrix(Vector2Int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        Vector2Int[,] rotated = new Vector2Int[cols, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                rotated[x, rows - 1 - y] = matrix[y, x];
            }
        }

        return rotated;
    }
}