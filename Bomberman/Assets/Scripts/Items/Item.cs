using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Bomb,
        Power,
        Speed,
        WallPass,
        Invincible,
        FlameImmune
    }
    
    public ItemType itemType;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectItem(this);
            }
        }
    }
}