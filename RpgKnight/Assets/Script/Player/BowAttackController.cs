using UnityEngine;

[RequireComponent(typeof(BowShooter))]
public class BowAttackController : MonoBehaviour
{
    [SerializeField] private BowShooter bowShooter;
    [SerializeField] private Animator animator;
    [SerializeField] private string isAttackingParam = "IsAttacking";
    [SerializeField] private string attackSpeedParam = "AttackSpeed";
    [SerializeField] private float attackSpeed = 1f;

    private bool attackInProgress;
    private Vector2? pendingShootDirection;

    public bool IsAttackInProgress => attackInProgress;
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = Mathf.Max(0.01f, value);
    }

    private void Awake()
    {
        if (bowShooter == null)
            bowShooter = GetComponent<BowShooter>();
    }

    public bool TryStartAttack(Vector2? overrideDirection = null)
    {
        if (attackInProgress || bowShooter == null || !bowShooter.IsReadyToShoot)
            return false;

        pendingShootDirection = overrideDirection;

        if (animator == null)
        {
            attackInProgress = true;
            FireArrowAttack();
            FinishArrowAttack();
            return true;
        }

        attackInProgress = true;
        animator.SetFloat(attackSpeedParam, attackSpeed);
        animator.SetBool(isAttackingParam, true);
        return true;
    }

    public void FireArrowAttack()
    {
        if (bowShooter == null)
            return;

        bowShooter.TryShoot(transform, pendingShootDirection);
        pendingShootDirection = null;
    }

    public void FinishArrowAttack()
    {
        attackInProgress = false;

        if (animator == null)
            return;

        animator.SetBool(isAttackingParam, false);
        animator.SetFloat(attackSpeedParam, 1f);
    }
}
