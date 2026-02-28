using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxBombs = 1;
    public int bombPower = 1;
    public int currentBombs = 0;
    public bool canPassWalls = false;
    public int lives = 3;
    public bool isInvincible = false;
    public float invincibilityDuration = 3f;
    private float invincibilityTimer = 0f;
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    
    public GameObject bombPrefab;
    
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update() 
    {
        HandleInput();
        Move();
        
        // 处理无敌状态
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                // 恢复正常渲染
                GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                // 无敌状态闪烁效果
                float alpha = Mathf.PingPong(Time.time * 10, 1f);
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            }
        }
    }
    
    void HandleInput() 
    {
        // 处理移动输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        
        // 处理放置炸弹
        if (Input.GetKeyDown(KeyCode.Space) && currentBombs < maxBombs)
        {
            PlaceBomb();
        }
        
        // 处理暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.TogglePause();
        }
    }
    
    void Move() 
    {
        rb.velocity = moveDirection * moveSpeed;
    }
    
    void PlaceBomb() 
    {
        // 计算炸弹放置位置（网格对齐）
        Vector2 bombPosition = new Vector2(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y)
        );
        
        // 实例化炸弹
        GameObject bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
        
        // 设置炸弹属性
        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.power = bombPower;
            bombScript.owner = this;
        }
        
        currentBombs++;
    }
    
    public void BombExploded()
    {
        currentBombs--;
    }
    
    public void CollectItem(Item item) 
    {
        switch (item.itemType)
        {
            case Item.ItemType.Bomb:
                maxBombs++;
                break;
            case Item.ItemType.Power:
                bombPower++;
                break;
            case Item.ItemType.Speed:
                moveSpeed += 1f;
                break;
            case Item.ItemType.WallPass:
                canPassWalls = true;
                break;
            case Item.ItemType.Invincible:
                StartInvincibility();
                break;
            case Item.ItemType.FlameImmune:
                // 实现火焰免疫逻辑
                break;
        }
        
        // 销毁道具
        Destroy(item.gameObject);
    }
    
    public void TakeDamage() 
    {
        if (!isInvincible)
        {
            lives--;
            
            if (lives <= 0)
            {
                // 玩家死亡
                Die();
            }
            else
            {
                // 触发无敌状态
                StartInvincibility();
            }
        }
    }
    
    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }
    
    void Die()
    {
        // 通知GameManager玩家死亡
        GameManager.instance.PlayerDied();
        
        // 销毁玩家对象
        Destroy(gameObject);
    }
    
    // 处理与障碍物的碰撞
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("UnbreakableWall"))
        {
            // 不可穿过的墙
        }
        else if (collision.gameObject.CompareTag("BreakableWall") && !canPassWalls)
        {
            // 可破坏的墙，除非有穿墙能力
        }
    }
    
    // 处理与出口的碰撞
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            // 关卡完成
            GameManager.instance.LevelComplete();
        }
    }
}