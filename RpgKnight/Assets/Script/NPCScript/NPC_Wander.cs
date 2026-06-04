using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Wander : MonoBehaviour
{
    [Header("Wander Area")]
    public float wanderWidth = 5;
    public float wanderHeight = 5;
    public Vector2 startingPosition;
    private Rigidbody2D rb;
    public Vector2 target;
    public float speed = 2f;
    public float pauseDuration = 1f;
    private bool isPaused = false;
    private Animator animator;

private void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponentInChildren<Animator>();
}

private void OnEnable()
{
    StartCoroutine(PauseWander());
}

private void Update()
{
    if (!isPaused)
    {
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(PauseWander());
        }
        Move();
    }
    else
    {
        rb.velocity = Vector2.zero;
    }
}

    private void Move()
    {
        Vector2 direction = ((Vector3)target - transform.position).normalized;
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                }
        rb.velocity = direction * speed;
    }

    IEnumerator PauseWander()
    {
        isPaused = true;
        animator.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);
        target = GetRandomTarget();
        isPaused = false;
        animator.Play("Walk");
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 碰撞时移动到起始位置
        target = startingPosition;
        if(!enabled){
            return;
        }
        StartCoroutine(PauseWander());
    }

private Vector2 GetRandomTarget()
{

    float halfWidth = wanderWidth / 2;
    float halfHeight = wanderHeight / 2;
    int edge = Random.Range(0, 4);

    return edge switch
    {
        0 => new Vector2(startingPosition.x - halfWidth, Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)),
        1 => new Vector2(startingPosition.x + halfWidth, Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)),
        2 => new Vector2(Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth), startingPosition.y - halfHeight),
        _ => new Vector2(Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth), startingPosition.y + halfHeight),
        
    };

}


private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(startingPosition, new Vector3(wanderWidth, wanderHeight, 0));
}
}
