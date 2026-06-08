using System.Collections;
using UnityEngine;

public class BossEnrage : MonoBehaviour
{
    [Header("触发设置")]
    [Range(0f, 1f)] public float healthThreshold = 0.5f;
    public bool hasEnraged = false;

    [Header("体型变化")]
    public float scaleMultiplier = 2f;
    public float enrageDuration = 2f;

    [Header("攻击范围")]
    public float enrageAttackRange = 4f;

    [Header("速度增加")]
    public float enrageSpeed = 2f;

    private Enemy_Health bossHealth;
    private EnemyMovement enemyMovement;

    private void Start()
    {
        bossHealth = GetComponent<Enemy_Health>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (bossHealth != null)
            bossHealth.OnHealthChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (hasEnraged || bossHealth == null) return;

        if ((float)currentHealth / maxHealth <= healthThreshold)
        {
            Enrage();
        }
    }

    private void Enrage()
    {
        hasEnraged = true;

        if (enemyMovement != null)
        {
            enemyMovement.attackRange = enrageAttackRange;
            enemyMovement.moveSpeed = enrageSpeed;
            enemyMovement.ChangeState(EnemyState.Defensing);
        }

        StartCoroutine(ScaleUpRoutine());
    }

    private IEnumerator ScaleUpRoutine()
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * scaleMultiplier;
        float elapsed = 0f;

        while (elapsed < enrageDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / enrageDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;

        if (enemyMovement != null)
        {
            enemyMovement.ChangeState(EnemyState.Chasing);
        }
    }
}
