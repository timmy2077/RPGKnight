using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform player;
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float attackCooldownTimer = 0f;
    private bool isAttackCooldown = false;
    private bool isKnockedBack = false;
    private int facingDirection = -1;
    private Animator anim;
    public EnemyState enemyState;

    private Vector3 spawnPosition;

    [Header("Boss设置")]
    public bool isBoss = false;
    public Collider2D bossDetectionTrigger;
    public GameObject bossUI;

    [Header("攻击位移")]
    public bool attackMovement = false;
    public float attackMoveSpeed = 3f;

    [Header("力竭")]
    public bool tireAfterAttack = false;
    public float tireDuration = 3f;
    private float tireTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spawnPosition = transform.position;

        if (isBoss && bossDetectionTrigger != null)
        {
            BossDetectionZone zone = bossDetectionTrigger.GetComponent<BossDetectionZone>();
            if (zone == null)
            {
                zone = bossDetectionTrigger.gameObject.AddComponent<BossDetectionZone>();
            }
            zone.Initialize(this);
        }

        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isKnockedBack)
        {
            return;
        }
        else if (enemyState == EnemyState.Returning)
        {
            ReturnToSpawn();
        }
        else if (isAttackCooldown)
        {
            if (enemyState == EnemyState.Attacking && attackMovement)
            {
                rb.velocity = new Vector2(facingDirection * attackMoveSpeed, rb.velocity.y);
            }

            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                isAttackCooldown = false;
                if (tireAfterAttack)
                {
                    ChangeState(EnemyState.Tiring);
                }
                else if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
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
        else if (enemyState == EnemyState.Tiring)
        {
            rb.velocity = Vector2.zero;
            tireTimer -= Time.deltaTime;
            if (tireTimer <= 0)
            {
                if (player != null)
                {
                    ChangeState(EnemyState.Chasing);
                }
                else
                {
                    ChangeState(EnemyState.Idle);
                }
            }
        }
        else if (enemyState == EnemyState.Defensing)
        {
            rb.velocity = Vector2.zero;
        }
        else if (enemyState == EnemyState.Dead)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Returning);
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
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

    void ReturnToSpawn()
    {
        float distToSpawn = Vector2.Distance(transform.position, spawnPosition);

        if (distToSpawn <= 0.1f)
        {
            transform.position = spawnPosition;
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }

        Vector2 direction = ((Vector2)spawnPosition - (Vector2)transform.position).normalized;

        if (spawnPosition.x > transform.position.x && facingDirection == -1 ||
            spawnPosition.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        rb.velocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBoss) return;

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
        if (isBoss) return;

        if (collision.CompareTag("Player"))
        {
            player = null;
            ChangeState(EnemyState.Returning);
        }
    }

    public void OnBossDetectPlayer(Transform playerTransform)
    {
        player = playerTransform;
        if (bossUI != null) bossUI.SetActive(true);
        ChangeState(EnemyState.Chasing);
    }

    public void OnBossLosePlayer()
    {
        player = null;
        if (bossUI != null) bossUI.SetActive(false);
        ChangeState(EnemyState.Returning);
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }

    public void ChangeState(EnemyState newState)
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
        else if (enemyState == EnemyState.Returning)
        {
            anim.SetBool("IsWalking", false);
        }
        else if (enemyState == EnemyState.Tiring)
        {
            anim.SetBool("IsTiring", false);
        }
        else if (enemyState == EnemyState.Defensing)
        {
            anim.SetBool("IsDefensed", false);
        }
        else if (enemyState == EnemyState.Dead)
        {
            anim.SetBool("IsDead", false);
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
        else if (enemyState == EnemyState.Returning)
        {
            anim.SetBool("IsWalking", true);
        }
        else if (enemyState == EnemyState.Tiring)
        {
            anim.SetBool("IsTiring", true);
            tireTimer = tireDuration;
        }
        else if (enemyState == EnemyState.Defensing)
        {
            anim.SetBool("IsDefensed", true);
        }
        else if (enemyState == EnemyState.Dead)
        {
            anim.SetBool("IsDead", true);
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
        spawnPosition = transform.position;

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
    Returning,
    Tiring,
    Defensing,
    Dead,
}
