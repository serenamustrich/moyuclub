using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public enum EnemyType
    {
        ArmoredCar,    // 装甲车
        LightTank,     // 轻型坦克
        AntiTankGun,   // 反坦克炮
        HeavyTank      // 重型坦克
    }

    public EnemyType enemyType;
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 2f;
    public float changeDirectionTime = 2f;
    public int health = 1;
    public int scoreValue = 100;

    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private float nextDirectionChangeTime = 0f;
    private int currentDirection = 0;
    private float moveInput = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextDirectionChangeTime = Time.time + changeDirectionTime;
        currentDirection = Random.Range(0, 4);
        
        // 根据敌人类型设置属性
        SetEnemyProperties();
    }

    private void SetEnemyProperties()
    {
        switch (enemyType)
        {
            case EnemyType.ArmoredCar:
                moveSpeed = 4f;
                fireRate = 3f;
                health = 1;
                scoreValue = 100;
                break;
            case EnemyType.LightTank:
                moveSpeed = 2f;
                fireRate = 2f;
                health = 1;
                scoreValue = 200;
                break;
            case EnemyType.AntiTankGun:
                moveSpeed = 1.5f;
                fireRate = 1.5f;
                health = 2;
                scoreValue = 300;
                break;
            case EnemyType.HeavyTank:
                moveSpeed = 1f;
                fireRate = 1f;
                health = 3;
                scoreValue = 400;
                break;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        // 定期改变方向
        if (Time.time > nextDirectionChangeTime)
        {
            currentDirection = Random.Range(0, 4);
            nextDirectionChangeTime = Time.time + changeDirectionTime;
        }

        // 根据当前方向旋转
        Quaternion targetRotation = Quaternion.Euler(0, 0, -90f * currentDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 向前移动
        Vector2 movement = new Vector2(0, moveInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);
    }

    private void HandleShooting()
    { 
        if (Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            // 碰撞时改变方向
            currentDirection = (currentDirection + 1) % 4;
            nextDirectionChangeTime = Time.time + changeDirectionTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.AddScore(scoreValue);
        // 有一定几率生成宝物
        if (Random.value < 0.2f)
        {
            // 生成宝物
        }
        Destroy(gameObject);
    }
}
