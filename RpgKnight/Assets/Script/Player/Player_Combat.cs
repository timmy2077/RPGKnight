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


    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
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

    public void FinishAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

}
