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
    private bool hasFoundTarget = false;
    // public LayerMask enemyLayer;
    public int damage=1;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    private ObjectPool pool;           // 所属的对象池（可选，用于回收）

    void Start()
    {
        // rb.velocity=direction*speed;
        // RotateArrow();
        // // 使用协程而不是Destroy，以便可以在返回对象池时停止
        
        FindClosestEnemy();
        StartCoroutine(LifeTimer());
    }

    // 当子弹从池中取出时调用，初始化状态
public void OnSpawn(ObjectPool ownerPool)
{
    pool = ownerPool;
    // 重置追踪状态
    hasFoundTarget = false;
    target = null;
    // 重新查找敌人
    FindClosestEnemy();
    // 可以设置初始旋转（可选）
    // transform.rotation = Quaternion.identity;
}
   void Update()
    {
        // 没找到敌人就直线飞
        if (target == null)
        {
            if (!hasFoundTarget)
            {
                FindClosestEnemy();
            }
            transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
            return;
        }

        // 追踪逻辑
        Vector2 dir = target.position - transform.position;
        dir.Normalize();

        float rotateAmount = Vector3.Cross(dir, transform.right).z;
        transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 寻找最近的敌人
    /// </summary>
    void FindClosestEnemy()
    {
        hasFoundTarget = true;

        // 找到所有标签为 Enemy 的物体
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0) return;

        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        // if ((enemyLayer.value & (1<<collision.gameObject.layer))>0)
        if (collision.gameObject.CompareTag("Enemy"))
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
