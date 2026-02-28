using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    private Vector2 moveDirection;
    private Vector2 nextDirection;
    private Rigidbody2D rb;
    
    // 长按检测变量
    private bool isKeyPressed = false;
    private float pressStartTime = 0f;
    private float longPressThreshold = 0.5f; // 长按阈值，单位：秒
    private bool isLongPress = false;
    private Vector2 pressedDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Vector2.zero;
        nextDirection = Vector2.zero;
        isKeyPressed = false;
        isLongPress = false;
        hasMoved = false;
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused || !GameManager.instance.isGameStarted)
            return;

        // 处理输入
        HandleInput();

        // 更新动画
        UpdateAnimation();
    }

    private void HandleInput()
    {
        // 重置长按状态
        if (isKeyPressed && Time.time - pressStartTime > longPressThreshold)
        {
            isLongPress = true;
        }

        // 键盘输入
        bool keyPressed = false;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            HandleKeyDown(Vector2.up);
            keyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            HandleKeyDown(Vector2.down);
            keyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HandleKeyDown(Vector2.left);
            keyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleKeyDown(Vector2.right);
            keyPressed = true;
        }
        
        // 检测按键释放
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            HandleKeyUp();
        }

        // 触摸输入（移动端）- 暂时注释掉，因为我们使用MobileInputController来处理触摸输入
        // if (Input.touchCount > 0)
        // {
        //     Touch touch = Input.GetTouch(0);
        //     if (touch.phase == TouchPhase.Began)
        //     {
        //         HandleKeyDown(GetTouchDirection(touch));
        //     }
        //     else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        //     {
        //         HandleKeyUp();
        //     }
        // }
    }

    private Vector2 GetTouchDirection(Touch touch)
    {
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        Vector2 pacmanPos = transform.position;
        Vector2 direction = touchPos - pacmanPos;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? Vector2.right : Vector2.left;
        else
            return direction.y > 0 ? Vector2.up : Vector2.down;
    }

    public void HandleKeyDown(Vector2 direction)
    {
        nextDirection = direction;
        pressedDirection = direction;
        isKeyPressed = true;
        pressStartTime = Time.time;
        isLongPress = false;
        hasMoved = false; // 重置移动状态
    }

    public void HandleKeyUp()
    {
        isKeyPressed = false;
        isLongPress = false;
        hasMoved = false; // 重置移动状态
    }

    // 移动控制变量
    private bool hasMoved = false;
    private float moveDistance = 1f; // 每个格子的距离
    private Vector2 targetPosition;

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused || !GameManager.instance.isGameStarted)
        {
            // 确保游戏未开始或暂停时速度为零
            rb.velocity = Vector2.zero;
            return;
        }

        // 强制重置速度，确保每次物理更新都从静止开始
        rb.velocity = Vector2.zero;

        // 按键释放，重置所有状态
        if (!isKeyPressed)
        {
            hasMoved = false;
            moveDirection = Vector2.zero;
            nextDirection = Vector2.zero;
            isLongPress = false;
            return;
        }

        // 只有在按键按下且有有效方向时才尝试改变方向
        if (nextDirection != Vector2.zero && CanMove(nextDirection))
        {
            moveDirection = nextDirection;
        }

        // 如果没有有效方向，直接返回
        if (moveDirection == Vector2.zero)
        {
            return;
        }

        // 移动逻辑：按一下动一下，长按连续动
        if (isLongPress)
        {
            // 长按状态：连续移动
            if (CanMove(moveDirection))
            {
                rb.velocity = moveDirection * moveSpeed;
            }
        }
        else if (!hasMoved)
        {
            // 短按状态：按一下动一下
            if (CanMove(moveDirection))
            {
                // 计算目标位置
                targetPosition = (Vector2)transform.position + moveDirection * moveDistance;
                // 移动到目标位置
                rb.MovePosition(targetPosition);
                hasMoved = true;
            }
        }
    }

    private bool CanMove(Vector2 direction)
    {
        // 检测前方是否有墙壁
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);
        return hit.collider == null || !hit.collider.CompareTag("Wall");
    }

    private void UpdateAnimation()
    {
        if (moveDirection == Vector2.up)
            animator.SetFloat("DirX", 0);
        else if (moveDirection == Vector2.down)
            animator.SetFloat("DirX", 0);
        else if (moveDirection == Vector2.left)
            animator.SetFloat("DirX", -1);
        else if (moveDirection == Vector2.right)
            animator.SetFloat("DirX", 1);
    }

    public void SetDirection(Vector2 direction)
    {
        nextDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            GameManager.instance.AddScore(10);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerFood"))
        {
            GameManager.instance.AddScore(50);
            Destroy(other.gameObject);
            // 使幽灵害怕
            GhostController[] ghosts = FindObjectsOfType<GhostController>();
            foreach (GhostController ghost in ghosts)
            {
                ghost.BecomeAfraid();
            }
        }
        else if (other.CompareTag("Ghost"))
        {
            GhostController ghost = other.GetComponent<GhostController>();
            if (ghost.isAfraid)
            {
                GameManager.instance.AddScore(200);
                ghost.ResetPosition();
            }
            else
            {
                GameManager.instance.GameOver();
            }
        }
    }
}