using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public int flip = 1;
    public float lastHorizontal = 0;
    private bool isKnockedBack;
    public Player_Combat player_Combat;
    public static PlayerMovement instance;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Slash") && player_Combat.enabled==true)
        {
            player_Combat.Attack();
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack==false)
        {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical"); 
        Vector2 inputDirection = new Vector2(horizontal, vertical);
        if (inputDirection.magnitude > 1)
        {
            inputDirection = inputDirection.normalized;
        }
        if (horizontal != 0)
        {
            lastHorizontal = horizontal;
            if (horizontal < 0)
            {
                flip = -1;
            }
            else
            {
                flip = 1;
            }
        }
        else if (horizontal == 0 && vertical == 0)
        {
            // 停止移动时保持最后一次的朝向
            if (lastHorizontal < 0)
            {
                flip = -1;
            }
            else if (lastHorizontal > 0)
            {
                flip = 1;
            }
        }
        transform.localScale = new Vector3(flip, 1, 1);
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
        rb.velocity = inputDirection * StatsManager.Instance.moveSpeed;
        }
    }  

    public void Knocback(Transform enemy, float force,float cooldown)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCooldown(cooldown));
    }

    IEnumerator KnockbackCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        // rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
