using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int ExpReward = 3;
    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;
    
    public int currentHealth;
    public int maxHealth;
    public GameObject enemyPrefab;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        else if (currentHealth <= 0)
        {
            if(OnMonsterDefeated != null)
            {
                OnMonsterDefeated(ExpReward);
            }
            
            if (enemyPrefab != null && PoolManager.Instance != null)
            {
                PoolManager.Instance.ReturnObject(gameObject, enemyPrefab);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnSpawn()
    {
        currentHealth = maxHealth;
    }
}