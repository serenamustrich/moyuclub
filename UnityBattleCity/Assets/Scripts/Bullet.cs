using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public string bulletType = "PlayerBullet";
    public int damage = 1;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 设置子弹速度
        rb.velocity = transform.up * speed;
        // 2秒后自动销毁
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 子弹碰撞检测
        if (bulletType == "PlayerBullet")
        {
            // 玩家子弹击中敌人
            if (collision.CompareTag("Enemy"))
            {
                EnemyTank enemy = collision.GetComponent<EnemyTank>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
            // 玩家子弹击中墙壁
            else if (collision.CompareTag("Wall"))
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            // 玩家子弹击中钢板
            else if (collision.CompareTag("Steel"))
            {
                Destroy(gameObject);
            }
            // 玩家子弹击中基地
            else if (collision.CompareTag("Base"))
            {
                // 基地被击中，游戏结束
                GameManager.instance.GameOver();
                Destroy(gameObject);
            }
        }
        else if (bulletType == "EnemyBullet")
        {
            // 敌人子弹击中人
            if (collision.CompareTag("Player"))
            {
                PlayerTank player = collision.GetComponent<PlayerTank>();
                if (player != null)
                {
                    player.Die();
                }
                Destroy(gameObject);
            }
            // 敌人子弹击中墙壁
            else if (collision.CompareTag("Wall"))
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            // 敌人子弹击中钢板
            else if (collision.CompareTag("Steel"))
            {
                Destroy(gameObject);
            }
            // 敌人子弹击中基地
            else if (collision.CompareTag("Base"))
            {
                // 基地被击中，游戏结束
                GameManager.instance.GameOver();
                Destroy(gameObject);
            }
        }
        
        // 子弹互相抵消
        if ((bulletType == "PlayerBullet" && collision.CompareTag("EnemyBullet")) ||
            (bulletType == "EnemyBullet" && collision.CompareTag("PlayerBullet")))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 处理碰撞
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Steel"))
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                Destroy(collision.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
