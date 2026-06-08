using UnityEngine;

public class BossSkillHitbox : MonoBehaviour
{
    public int damage = 5;
    public float detectionRadius = 2f;

    private bool hasDealtDamage = false;

    public void DealDamage()
    {
        if (hasDealtDamage) return;
        hasDealtDamage = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ChangeHealth(-damage);
                }
            }
        }
    }

    public void ResetDamage()
    {
        hasDealtDamage = false;
    }
}
