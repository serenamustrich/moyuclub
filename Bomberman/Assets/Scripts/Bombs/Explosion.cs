using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float duration = 0.5f;
    private float timer = 0f;
    
    void Start()
    {
        timer = duration;
    }
    
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }
        else if (other.CompareTag("Bomb"))
        {
            Bomb bomb = other.GetComponent<Bomb>();
            if (bomb != null)
            {
                bomb.TriggerExplosion();
            }
        }
    }
}