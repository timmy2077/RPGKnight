using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockedBack;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Knockback(Transform playerTransform, float knockbackForce, float cooldown)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            // 通知EnemyMovement脚本敌人被击退
            EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.SetKnockedBack(true);
            }
            Vector2 direction = (transform.position - playerTransform.position).normalized;
            rb.velocity = direction * knockbackForce;
            StartCoroutine(KnockbackCooldown(cooldown));
        }
    }

    IEnumerator KnockbackCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isKnockedBack = false;
        // 通知EnemyMovement脚本敌人击退结束
        EnemyMovement enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.SetKnockedBack(false);
        }
    }
}
