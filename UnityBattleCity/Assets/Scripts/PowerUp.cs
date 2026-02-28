using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        ExtraLife,      // 额外生命
        SpeedBoost,     // 速度提升
        RapidFire,      // 快速开火
        Shield,         // 护盾
        Grenade         // 手榴弹（清屏）
    }

    public PowerUpType powerUpType;
    public float effectDuration = 10f;

    public void ApplyEffect(GameObject target)
    {
        switch (powerUpType)
        {
            case PowerUpType.ExtraLife:
                GameManager.instance.playerLives++;
                GameManager.instance.UpdateUI();
                break;
            case PowerUpType.SpeedBoost:
                PlayerTank player = target.GetComponent<PlayerTank>();
                if (player != null)
                {
                    StartCoroutine(SpeedBoostEffect(player));
                }
                break;
            case PowerUpType.RapidFire:
                PlayerTank player2 = target.GetComponent<PlayerTank>();
                if (player2 != null)
                {
                    StartCoroutine(RapidFireEffect(player2));
                }
                break;
            case PowerUpType.Shield:
                PlayerTank player3 = target.GetComponent<PlayerTank>();
                if (player3 != null)
                {
                    StartCoroutine(ShieldEffect(player3));
                }
                break;
            case PowerUpType.Grenade:
                ClearEnemies();
                break;
        }
    }

    private System.Collections.IEnumerator SpeedBoostEffect(PlayerTank player)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed *= 1.5f;
        yield return new WaitForSeconds(effectDuration);
        player.moveSpeed = originalSpeed;
    }

    private System.Collections.IEnumerator RapidFireEffect(PlayerTank player)
    {
        float originalFireRate = player.fireRate;
        player.fireRate *= 0.5f;
        yield return new WaitForSeconds(effectDuration);
        player.fireRate = originalFireRate;
    }

    private System.Collections.IEnumerator ShieldEffect(PlayerTank player)
    {
        // 实现护盾效果
        // 这里可以添加护盾视觉效果和碰撞检测
        yield return new WaitForSeconds(effectDuration);
    }

    private void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
            GameManager.instance.AddScore(100); // 每个敌人100分
        }
    }
}
