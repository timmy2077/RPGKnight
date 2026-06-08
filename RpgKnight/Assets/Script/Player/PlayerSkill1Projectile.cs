using UnityEngine;

public class PlayerSkill1Projectile : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int damage = 2;
    public float lifeTime = 3f;

    private float direction;
    private Rigidbody2D rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float facingDirection)
    {
        direction = facingDirection;
        transform.localScale = new Vector3(facingDirection, 1f, 1f);
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            Enemy_Health enemyHealth = other.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
            }
            Destroy(gameObject);
        }
    }
}
