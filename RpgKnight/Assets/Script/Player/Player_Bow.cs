using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform launchPoint;
    private Vector2 mouseDirection=Vector2.right;
    private float shootTimer;
    public float shootCooldown=0.5f;
    public float detectionRange = 10f;
    private Transform targetEnemy;

    // public Animator anim;
    
    void Start()
    {
        // 确保池子已创建（可在游戏启动时统一初始化）
        PoolManager.Instance.CreatePool(arrowPrefab, 5, 10);
        shootTimer = shootCooldown;

    }



    void Update()
    {
        shootTimer -= Time.deltaTime;
        FindClosestEnemy();
        AimAtEnemy();
        if (Input.GetButtonDown("Shoot") && shootTimer <= 0 && targetEnemy != null)
        {
            // anim.SetBool("IsShooting",true);
            Shoot();
        }
    }
    

    public void Shoot()
{
    // 从池中获取子弹
    GameObject arrow = PoolManager.Instance.GetObject(arrowPrefab);
    if (arrow == null) return;

    // 设置位置
    arrow.transform.position = launchPoint.position;
    
    // 设置初始旋转（朝向发射方向）
    float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
    arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    // 翻转玩家面向敌人（通过修改PlayerMovement的flip和lastHorizontal变量保持状态）
    if (PlayerMovement.instance != null)
    {
        if (mouseDirection.x < 0)
        {
            PlayerMovement.instance.flip = -1;
            PlayerMovement.instance.lastHorizontal = -1;
            Debug.Log("Flip Left");
        }
        else
        {
            PlayerMovement.instance.flip = 1;
            PlayerMovement.instance.lastHorizontal = 1;
            Debug.Log("Flip Right");
        }
    }

    // 子弹脚本中OnSpawn已调用，会设置lifeTime等
    shootTimer = shootCooldown;
}

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemies.Length == 0)
        {
            targetEnemy = null;
            return;
        }

        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist && dist <= detectionRange)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    void AimAtEnemy()
    {
        if (targetEnemy == null)
        {
            mouseDirection = Vector2.right;
            return;
        }

        Vector2 direction = targetEnemy.position - launchPoint.position;
        direction.Normalize();
        mouseDirection = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        launchPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            mouseDirection = new Vector2(horizontal, vertical).normalized;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (targetEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(launchPoint.position, targetEnemy.position);
            Gizmos.DrawWireSphere(targetEnemy.position, 0.5f);
        }
    }
}
