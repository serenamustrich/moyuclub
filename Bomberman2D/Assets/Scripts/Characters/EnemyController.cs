using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Normal,      // 普通敌人：慢，随机移动
        Fast,        // 快速敌人：快，追踪玩家
        Flying,      // 飞行敌人：中等，穿过障碍物
        Exploding    // 爆炸敌人：慢，随机移动，死亡时爆炸
    }
    
    public EnemyType enemyType;
    public float moveSpeed = 2f;
    public int health = 1;
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float changeDirectionTime = 2f;
    private float timer;
    private GameObject player;
    
    // 爆炸敌人相关
    public GameObject explosionPrefab;
    public int explosionPower = 2;
    
    // 飞行敌人相关
    public bool canFly = false;
    
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        timer = changeDirectionTime;
        
        // 根据敌人类型设置属性
        switch (enemyType)
        {
            case EnemyType.Normal:
                moveSpeed = 1.5f;
                break;
            case EnemyType.Fast:
                moveSpeed = 3f;
                break;
            case EnemyType.Flying:
                moveSpeed = 2f;
                canFly = true;
                break;
            case EnemyType.Exploding:
                moveSpeed = 1f;
                break;
        }
    }
    
    void Update() 
    {
        Move();
    }
    
    void Move() 
    {
        timer -= Time.deltaTime;
        
        switch (enemyType)
        {
            case EnemyType.Normal:
            case EnemyType.Exploding:
                // 随机移动
                if (timer <= 0)
                {
                    ChangeDirectionRandomly();
                    timer = changeDirectionTime;
                }
                break;
                
            case EnemyType.Fast:
                // 追踪玩家
                if (player != null)
                {
                    moveDirection = (player.transform.position - transform.position).normalized;
                }
                else
                {
                    // 玩家不存在时随机移动
                    if (timer <= 0)
                    {
                        ChangeDirectionRandomly();
                        timer = changeDirectionTime;
                    }
                }
                break;
                
            case EnemyType.Flying:
                // 飞行敌人：可以穿过障碍物，随机移动
                if (timer <= 0)
                {
                    ChangeDirectionRandomly();
                    timer = changeDirectionTime;
                }
                break;
        }
        
        rb.velocity = moveDirection * moveSpeed;
    }
    
    void ChangeDirectionRandomly()
    {
        // 随机选择四个方向之一
        int randomDirection = Random.Range(0, 4);
        
        switch (randomDirection)
        {
            case 0:
                moveDirection = Vector2.up;
                break;
            case 1:
                moveDirection = Vector2.down;
                break;
            case 2:
                moveDirection = Vector2.left;
                break;
            case 3:
                moveDirection = Vector2.right;
                break;
        }
    }
    
    public void TakeDamage() 
    {
        health--;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // 通知GameManager敌人死亡
        GameManager.instance.EnemyKilled();
        
        // 爆炸敌人特殊处理
        if (enemyType == EnemyType.Exploding)
        {
            CreateExplosion();
        }
        
        // 销毁敌人对象
        Destroy(gameObject);
    }
    
    void CreateExplosion()
    {
        if (explosionPrefab != null)
        {
            // 中心爆炸
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            // 向四个方向生成爆炸
            for (int i = 1; i <= explosionPower; i++)
            {
                // 向上
                Instantiate(explosionPrefab, transform.position + Vector3.up * i, Quaternion.identity);
                // 向下
                Instantiate(explosionPrefab, transform.position + Vector3.down * i, Quaternion.identity);
                // 向左
                Instantiate(explosionPrefab, transform.position + Vector3.left * i, Quaternion.identity);
                // 向右
                Instantiate(explosionPrefab, transform.position + Vector3.right * i, Quaternion.identity);
            }
        }
    }
    
    // 处理碰撞
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canFly) // 飞行敌人可以穿过障碍物
        {
            if (collision.gameObject.CompareTag("UnbreakableWall") || 
                collision.gameObject.CompareTag("BreakableWall"))
            {
                // 碰到墙时改变方向
                ChangeDirectionRandomly();
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                // 碰到玩家造成伤害
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage();
                }
            }
        }
    }
    
    // 飞行敌人的触发器碰撞
    void OnTriggerEnter2D(Collider2D other)
    {
        if (canFly)
        {
            if (other.CompareTag("Player"))
            {
                // 碰到玩家造成伤害
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage();
                }
            }
        }
    }
}