using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f;
    public AudioClip fireSound;

    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        // 键盘控制
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        // 移动
        Vector2 movement = new Vector2(0, moveVertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);

        // 旋转
        transform.Rotate(0, 0, -moveHorizontal * rotationSpeed * Time.deltaTime);

        // 移动设备控制
        if (MobileInputController.instance != null)
        {
            float mobileMove = MobileInputController.instance.GetMoveInput();
            float mobileRotate = MobileInputController.instance.GetRotateInput();

            // 移动
            Vector2 mobileMovement = new Vector2(0, mobileMove) * moveSpeed * Time.deltaTime;
            transform.Translate(mobileMovement, Space.Self);

            // 旋转
            transform.Rotate(0, 0, -mobileRotate * rotationSpeed * Time.deltaTime);

            // 射击
            if (MobileInputController.instance.GetFireInput() && Time.time > nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Die();
            Destroy(collision.gameObject);
        }
    }

    private void Die()
    {
        GameManager.instance.PlayerDied();
        Destroy(gameObject);
    }
}
