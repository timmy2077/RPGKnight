using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damageAmount = -1;
    public Transform attackPoint;
    public float weponRange;
    public LayerMask playerLayer;
    public float knockbackForce = 5f;
    public float knockbackCooldown = 0.5f;

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
    //         playerHealth.ChangeHealth(damageAmount);
    //     }
    // }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weponRange, playerLayer);
        if(hits.Length > 0)
        {
             hits[0].gameObject.GetComponent<PlayerHealth>().ChangeHealth(damageAmount);
             hits[0].gameObject.GetComponent<PlayerMovement>().Knocback(transform, knockbackForce, knockbackCooldown);
        }
    }
}
