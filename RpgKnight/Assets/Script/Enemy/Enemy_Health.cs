using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public EnemySO enemySO;
    public int ExpReward = 3;
    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;
    public delegate void HealthChanged(int currentHealth, int maxHealth);
    public event HealthChanged OnHealthChanged;
    
    public int currentHealth;
    public int maxHealth;
    public GameObject enemyPrefab;

    [Header("死亡动画")]
    public bool hasDeathAnimation = false;

    private EnemyMovement enemyMovement;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyMovement = GetComponent<EnemyMovement>();
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;
        if (amount < 0 && enemyMovement != null && enemyMovement.enemyState == EnemyState.Defensing) return;

        currentHealth += amount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;

            if (enemyMovement != null)
                enemyMovement.ChangeState(EnemyState.Dead);

            if (hasDeathAnimation)
            {
                // 等待动画事件调用 OnDeathAnimationEnd
            }
            else
            {
                Die();
            }
        }
    }

    public void OnDeathAnimationEnd()
    {
        Die();
    }

    private void Die()
    {
        if(OnMonsterDefeated != null)
        {
            OnMonsterDefeated(ExpReward);
        }

        if (enemySO != null)
            QuestEvents.OnEnemyKilled?.Invoke(enemySO);
        
        if (enemyPrefab != null && PoolManager.Instance != null)
        {
            PoolManager.Instance.ReturnObject(gameObject, enemyPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnSpawn()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}
