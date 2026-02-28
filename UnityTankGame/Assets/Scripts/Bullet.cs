using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public string bulletType = "player"; // player, enemy
    public float lifetime = 2f;
    
    private Rigidbody2D rb;
    private float spawnTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        
        // 设置子弹速度
        rb.velocity = transform.up * speed;
    }
    
    void Update()
    {
        // 检查子弹生命周期
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 玩家子弹碰撞
        if (bulletType == "player")
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // 击中敌人，销毁子弹
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                // 击中砖墙，销毁墙和子弹
                Destroy(collision.gameObject);
                Destroy(gameObject);
                // 播放爆炸音效
                AudioManager.Instance.PlaySound("WallExplosion");
            }
            else if (collision.gameObject.CompareTag("Steel"))
            {
                // 击中钢板，只销毁子弹
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Base"))
            {
                // 击中基地，游戏结束
                GameManager.Instance.GameOver();
                Destroy(gameObject);
            }
        }
        // 敌人子弹碰撞
        else if (bulletType == "enemy")
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // 击中玩家，游戏结束
                GameManager.Instance.GameOver();
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                // 击中砖墙，销毁墙和子弹
                Destroy(collision.gameObject);
                Destroy(gameObject);
                // 播放爆炸音效
                AudioManager.Instance.PlaySound("WallExplosion");
            }
            else if (collision.gameObject.CompareTag("Steel"))
            {
                // 击中钢板，只销毁子弹
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Base"))
            {
                // 击中基地，游戏结束
                GameManager.Instance.GameOver();
                Destroy(gameObject);
            }
        }
    }
}
