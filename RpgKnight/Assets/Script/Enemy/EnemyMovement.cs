using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform player;
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float attackCooldownTimer = 0f;
    private bool isAttackCooldown = false;
    private bool isKnockedBack = false;
    private int facingDirection = -1;
    private Animator anim;
    private EnemyState enemyState;
    public Collider2D enemyCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isKnockedBack)
        {
            // 被击退时停止移动
            return;
        }
        else if (isAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                isAttackCooldown = false;
                if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
                {
                    ChangeState(EnemyState.Attacking);
                }
                else if (player != null)
                {
                    ChangeState(EnemyState.Chasing);
                }
            }
        }
        else if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Chase()
    {
        if(Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
        }
        else if (player.position.x > transform.position.x && facingDirection == -1 || 
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
            
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null)
            {
                player = collision.transform;
            }
            ChangeState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            isAttackCooldown = false;
            attackCooldownTimer = 0f;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }

    private void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("IsIdle", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("IsWalking", false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("IsAttacking", false);
        }

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("IsIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("IsWalking", true);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("IsAttacking", true);
            StartAttackCooldown();
        }
    }

    private void StartAttackCooldown()
    {
        isAttackCooldown = true;
        attackCooldownTimer = attackCooldown;
    }

    public void SetKnockedBack(bool value)
    {
        isKnockedBack = value;
    }

    public void OnSpawn()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        isKnockedBack = false;
        isAttackCooldown = false;
        attackCooldownTimer = 0f;
        player = null;
        
        if (anim != null)
        {
            ChangeState(EnemyState.Idle);
        }
    }

}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}
