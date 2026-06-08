using UnityEngine;

public class BossDetectionZone : MonoBehaviour
{
    private EnemyMovement enemyMovement;

    public void Initialize(EnemyMovement movement)
    {
        enemyMovement = movement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enemyMovement != null)
        {
            enemyMovement.OnBossDetectPlayer(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enemyMovement != null)
        {
            enemyMovement.OnBossLosePlayer();
        }
    }
}
