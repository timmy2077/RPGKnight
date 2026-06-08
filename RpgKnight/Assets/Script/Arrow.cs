using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // public Rigidbody2D rb;
    // public Vector2 direction=Vector2.right;
    public float lifeSpawn=2f;
    public float speed=12f;
    public float rotateSpeed = 200f;
    private Transform target;
    private bool homingEnabled;
    private float homingRange;
    private Transform homingOrigin;
    private string enemyTag = "Enemy";
    // public LayerMask enemyLayer;
    public int damage=1;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    private ObjectPool pool;           // 所属的对象池（可选，用于回收）

    public void Initialize(bool enableHoming, float detectionRange, Transform origin, string targetTag = "Enemy")
    {
        homingEnabled = enableHoming;
        homingRange = detectionRange;
        homingOrigin = origin;
        enemyTag = string.IsNullOrEmpty(targetTag) ? "Enemy" : targetTag;
        target = null;

        if (homingEnabled)
            FindClosestEnemy();
    }

    public void OnSpawn(ObjectPool ownerPool)
    {
        pool = ownerPool;
        homingEnabled = false;
        homingOrigin = null;
        target = null;
        enemyTag = "Enemy";

        StopAllCoroutines();
        StartCoroutine(LifeTimer());
    }

    void Update()
    {
        if (homingEnabled)
        {
            if (target == null || !IsTargetInRange(target))
                FindClosestEnemy();

            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                dir.Normalize();

                float rotateAmount = Vector3.Cross(dir, transform.right).z;
                transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);
            }
        }

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
    }

    bool IsTargetInRange(Transform enemy)
    {
        if (homingOrigin == null || enemy == null)
            return false;

        return Vector2.Distance(homingOrigin.position, enemy.position) <= homingRange;
    }

    void FindClosestEnemy()
    {
        target = null;

        if (!homingEnabled || homingOrigin == null)
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
            return;

        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distFromPlayer = Vector2.Distance(homingOrigin.position, enemy.transform.position);
            if (distFromPlayer > homingRange)
                continue;

            float distFromArrow = Vector2.Distance(transform.position, enemy.transform.position);
            if (distFromArrow < closestDist)
            {
                closestDist = distFromArrow;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        // if ((enemyLayer.value & (1<<collision.gameObject.layer))>0)
        if (!string.IsNullOrEmpty(enemyTag) && collision.gameObject.CompareTag(enemyTag))
        {
            Enemy_Health enemyHealth = collision.gameObject.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
            }
            Enemy_Knockback enemyKnockback = collision.gameObject.GetComponent<Enemy_Knockback>();
            if (enemyKnockback != null)
            {
                enemyKnockback.Knockback(transform, knockbackForce, knockbackTime);
            }
             // 处理伤害等...
            ReturnToPool();

        }
    }

    // 生命周期定时器协程
    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeSpawn);
        ReturnToPool();
    }

     // 回收自己
    void ReturnToPool()
    {
        // 停止协程，防止重复返回
        StopCoroutine(LifeTimer());
        
        if (pool != null && gameObject != null)
        {
            pool.Return(gameObject);
            Debug.Log("ReturnToPool");
        }
        else if (gameObject != null)
        {
            // 如果没有池子（应急情况），直接销毁
            Destroy(gameObject);
        }
    }
}
