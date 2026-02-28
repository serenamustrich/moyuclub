using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Bomb,        // 增加炸弹数量
        Power,       // 增加炸弹威力
        Speed,       // 增加移动速度
        WallPass,    // 可以穿过墙壁
        Invincible,  // 无敌状态
        FlameImmune  // 火焰免疫
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