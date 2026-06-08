using UnityEngine;

[RequireComponent(typeof(BowShooter))]
[RequireComponent(typeof(BowAttackController))]
public class Player_Bow : MonoBehaviour
{
    [SerializeField] private BowShooter bowShooter;
    [SerializeField] private BowAttackController bowAttackController;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Awake()
    {
        if (bowShooter == null)
            bowShooter = GetComponent<BowShooter>();

        if (bowAttackController == null)
            bowAttackController = GetComponent<BowAttackController>();
    }

    private void Start()
    {
        if (PlayerMovement.instance != null)
            lastMoveDirection = new Vector2(PlayerMovement.instance.flip, 0f);
    }

    private void Update()
    {
        bowShooter.RefreshTarget();

        if (bowShooter.HasTargetInRange)
        {
            bowShooter.AimAtTarget();
        }
        else if (!bowAttackController.IsAttackInProgress)
        {
            UpdateMovementAimDirection();
            bowShooter.AimInDirection(lastMoveDirection);
        }

        if (Input.GetButtonDown("Shoot") && !bowAttackController.IsAttackInProgress)
        {
            Vector2? shootDirection = bowShooter.HasTargetInRange ? null : lastMoveDirection;
            if (bowAttackController.TryStartAttack(shootDirection))
                UpdatePlayerFlip();
        }
    }

    private void UpdateMovementAimDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(horizontal, vertical);

        if (inputDirection.sqrMagnitude > 0f)
            lastMoveDirection = inputDirection.normalized;
    }

    private void UpdatePlayerFlip()
    {
        if (PlayerMovement.instance == null)
            return;

        Vector2 aimDirection = bowShooter.AimDirection;
        if (aimDirection.x < 0f)
        {
            PlayerMovement.instance.flip = -1;
            PlayerMovement.instance.lastHorizontal = -1;
        }
        else
        {
            PlayerMovement.instance.flip = 1;
            PlayerMovement.instance.lastHorizontal = 1;
        }
    }
}
