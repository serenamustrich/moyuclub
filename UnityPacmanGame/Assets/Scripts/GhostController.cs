using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool isAfraid = false;
    public float afraidDuration = 10f;
    private float afraidTimer = 0f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Vector3 homePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = transform.position;
        SetRandomDirection();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused || !GameManager.instance.isGameStarted)
            return;

        if (isAfraid)
        {
            afraidTimer -= Time.deltaTime;
            if (afraidTimer <= 0)
            {
                isAfraid = false;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isGamePaused || !GameManager.instance.isGameStarted)
            return;

        // 尝试移动
        if (CanMove(moveDirection))
        {
            rb.velocity = moveDirection * moveSpeed * (isAfraid ? 0.7f : 1f);
        }
        else
        {
            SetRandomDirection();
        }
    }

    private bool CanMove(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);
        return hit.collider == null || !hit.collider.CompareTag("Wall");
    }

    private void SetRandomDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        int randomIndex = Random.Range(0, directions.Length);
        moveDirection = directions[randomIndex];
    }

    public void BecomeAfraid()
    {
        isAfraid = true;
        afraidTimer = afraidDuration;
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    public void ResetPosition()
    {
        transform.position = homePosition;
        isAfraid = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        SetRandomDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pacman"))
        {
            if (isAfraid)
            {
                GameManager.instance.AddScore(200);
                ResetPosition();
            }
            else
            {
                GameManager.instance.GameOver();
            }
        }
    }
}