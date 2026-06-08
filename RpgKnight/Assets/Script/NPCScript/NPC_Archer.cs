using UnityEngine;

[RequireComponent(typeof(BowShooter))]
[RequireComponent(typeof(BowAttackController))]
public class NPC_Archer : MonoBehaviour
{
    [SerializeField] private BowShooter bowShooter;
    [SerializeField] private BowAttackController bowAttackController;
    [SerializeField] private Transform visualRoot;

    private void Awake()
    {
        if (bowShooter == null)
            bowShooter = GetComponent<BowShooter>();

        if (bowAttackController == null)
            bowAttackController = GetComponent<BowAttackController>();

        if (visualRoot == null)
            visualRoot = transform;
    }

    private void Update()
    {
        bowShooter.RefreshTarget();

        if (bowShooter.HasTargetInRange)
        {
            bowShooter.AimAtTarget();
            FaceTarget(bowShooter.CurrentTarget);

            if (!bowAttackController.IsAttackInProgress && bowShooter.IsReadyToShoot)
                bowAttackController.TryStartAttack();
        }
    }

    private void FaceTarget(Transform target)
    {
        if (target == null || visualRoot == null)
            return;

        float directionX = target.position.x - visualRoot.position.x;
        if (Mathf.Approximately(directionX, 0f))
            return;

        Vector3 scale = visualRoot.localScale;
        scale.x = directionX < 0f ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        visualRoot.localScale = scale;
    }
}
