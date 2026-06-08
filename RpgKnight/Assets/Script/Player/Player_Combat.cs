using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator animator;
    public float coolDownTime = 2f;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    private float timer;

    [Header("技能1")]
    public GameObject skill1Prefab;
    public float skill1Cooldown = 3f;
    private float skill1Timer;

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (skill1Timer > 0)
        {
            skill1Timer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.G) && skill1Timer <= 0)
        // if (Input.GetButtonDown("Skill1") && skill1Timer <= 0)
        {
            Debug.Log("技能1");
            animator.SetBool("IsSkill", true);
            skill1Timer = skill1Cooldown;
        }
    }

    public void Attack()
    {
        if(timer <= 0)
        {
            animator.SetBool("IsAttacking", true);
            
            timer = coolDownTime;
        }
        
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
            if(hitEnemies.Length > 0)
            {
                hitEnemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.damage);
                hitEnemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime);
            }; 
    }

    public void SpawnSkill1()
    {
        if (skill1Prefab != null && attackPoint != null)
        {
            GameObject projectile = Instantiate(skill1Prefab, attackPoint.position, Quaternion.identity);
            PlayerSkill1Projectile skill = projectile.GetComponent<PlayerSkill1Projectile>();
            if (skill != null)
            {
                skill.SetDirection(transform.localScale.x);
            }
        }
    }

    public void FinishAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

    public void FinishSkill1()
    {
        animator.SetBool("IsSkill", false);
    }
}
