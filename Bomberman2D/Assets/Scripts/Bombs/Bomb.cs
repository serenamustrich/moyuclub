using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 3f;
    public int power = 1;
    public float explosionDuration = 0.5f;
    public PlayerController owner;
    
    private float countdown;
    private bool exploded = false;
    private SpriteRenderer spriteRenderer;
    
    public GameObject explosionPrefab;
    
    void Start() 
    {
        countdown = explosionDelay;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update() 
    {
        countdown -= Time.deltaTime;
        
        // 炸弹倒计时动画效果
        float scale = 1 + Mathf.Sin(Time.time * 10) * 0.1f;
        transform.localScale = new Vector3(scale, scale, 1);
        
        if (countdown <= 0 && !exploded) 
        {
            Explode();
        }
    }
    
    void Explode() 
    {
        exploded = true;
        
        // 生成爆炸效果
        CreateExplosions();
        
        // 通知所有者炸弹已爆炸
        if (owner != null)
        {
            owner.BombExploded();
        }
        
        // 销毁炸弹
        Destroy(gameObject);
    }
    
    void CreateExplosions()
    {
        // 中心爆炸
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        
        // 向上爆炸
        for (int i = 1; i <= power; i++)
        {
            Vector2 direction = Vector2.up * i;
            Vector2 position = (Vector2)transform.position + direction;
            
            // 检查是否碰到不可破坏的墙
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.2f);
            bool blocked = false;
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("UnbreakableWall"))
                {
                    blocked = true;
                    break;
                }
                else if (collider.CompareTag("BreakableWall"))
                {
                    // 破坏可破坏的墙
                    Destroy(collider.gameObject);
                    blocked = true;
                    break;
                }
            }
            
            if (blocked) break;
            
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        
        // 向下爆炸
        for (int i = 1; i <= power; i++)
        {
            Vector2 direction = Vector2.down * i;
            Vector2 position = (Vector2)transform.position + direction;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.2f);
            bool blocked = false;
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("UnbreakableWall"))
                {
                    blocked = true;
                    break;
                }
                else if (collider.CompareTag("BreakableWall"))
                {
                    Destroy(collider.gameObject);
                    blocked = true;
                    break;
                }
            }
            
            if (blocked) break;
            
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        
        // 向左爆炸
        for (int i = 1; i <= power; i++)
        {
            Vector2 direction = Vector2.left * i;
            Vector2 position = (Vector2)transform.position + direction;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.2f);
            bool blocked = false;
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("UnbreakableWall"))
                {
                    blocked = true;
                    break;
                }
                else if (collider.CompareTag("BreakableWall"))
                {
                    Destroy(collider.gameObject);
                    blocked = true;
                    break;
                }
            }
            
            if (blocked) break;
            
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        
        // 向右爆炸
        for (int i = 1; i <= power; i++)
        {
            Vector2 direction = Vector2.right * i;
            Vector2 position = (Vector2)transform.position + direction;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.2f);
            bool blocked = false;
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("UnbreakableWall"))
                {
                    blocked = true;
                    break;
                }
                else if (collider.CompareTag("BreakableWall"))
                {
                    Destroy(collider.gameObject);
                    blocked = true;
                    break;
                }
            }
            
            if (blocked) break;
            
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
    }
    
    // 处理其他炸弹的触发
    public void TriggerExplosion()
    {
        if (!exploded)
        {
            Explode();
        }
    }
}