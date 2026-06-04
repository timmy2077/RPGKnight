using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    public Vector2[] patrolPoints;
    private int currentPatrolIndex = 0;
    public Vector2 target;
    public float speed = 2f;
    private Rigidbody2D rb;
    public float pauseDuration = 1f;
    private bool isPaused = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            StartCoroutine(SetPatrolPoint());
        }
        else
        {
            Debug.LogWarning("Patrol points not set for " + gameObject.name);
            rb.velocity = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Vector2 direction = ((Vector3)target - transform.position).normalized;
                if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                }
                rb.velocity = direction * speed;

                if (Vector2.Distance(transform.position, target) < 0.1f)
                {
                    StartCoroutine(SetPatrolPoint());
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    IEnumerator SetPatrolPoint()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            isPaused = true;
            animator.Play("Idle");
            yield return new WaitForSeconds(pauseDuration); 
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            target = patrolPoints[currentPatrolIndex];
            isPaused = false;
            animator.Play("Walk");
        }
    }
}
