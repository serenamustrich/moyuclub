using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    
    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // 移动控制
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // 旋转坦克
        transform.Rotate(0, 0, -horizontal * rotationSpeed * Time.deltaTime);
        
        // 移动坦克
        Vector2 movement = transform.up * vertical * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        
        // 开火控制
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
        
        // 动画控制
        if (Mathf.Abs(vertical) > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
    
    void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        // 播放开火音效
        AudioManager.Instance.PlaySound("Fire");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 与敌人碰撞，游戏结束
            GameManager.Instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            // 被敌人子弹击中，游戏结束
            GameManager.Instance.GameOver();
        }
    }
}
