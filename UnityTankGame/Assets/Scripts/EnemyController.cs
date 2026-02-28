using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 90f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public string enemyType = "normal"; // normal, fast, smart, heavy
    
    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Transform player;
    private float changeDirectionTime = 0f;
    private float directionChangeInterval = 2f;
    private int direction = 0; // 0: up, 1: right, 2: down, 3: left
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // 根据敌人类型设置属性
        switch (enemyType)
        {
            case "fast":
                moveSpeed = 6f;
                fireRate = 1.5f;
                break;
            case "smart":
                moveSpeed = 4f;
                fireRate = 1f;
                directionChangeInterval = 1f;
                break;
            case "heavy":
                moveSpeed = 2f;
                fireRate = 2.5f;
                break;
        }
    }
    
    void Update()
    {
        // 定期改变方向
        if (Time.time >= changeDirectionTime)
        {
            ChangeDirection();
            changeDirectionTime = Time.time + directionChangeInterval;
        }
        
        // 智能敌人追踪玩家
        if (enemyType == "smart" && player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        // 移动敌人
        Vector2 movement = transform.up * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        
        // 开火
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void ChangeDirection()
    {
        if (enemyType != "smart")
        {
            direction = Random.Range(0, 4);
            transform.rotation = Quaternion.Euler(0, 0, -direction * 90f);
        }
    }
    
    void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        // 播放开火音效
        AudioManager.Instance.PlaySound("EnemyFire");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Steel") || collision.gameObject.CompareTag("Water"))
        {
            // 碰到障碍物，改变方向
            ChangeDirection();
        }
        else if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            // 被玩家子弹击中，销毁敌人
            Destroy(gameObject);
            // 播放爆炸音效
            AudioManager.Instance.PlaySound("Explosion");
            // 增加分数
            GameManager.Instance.AddScore(100);
        }
    }
}
