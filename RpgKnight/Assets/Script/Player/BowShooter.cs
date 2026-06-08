using UnityEngine;

public class BowShooter : MonoBehaviour
{
    [Header("Bow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private string targetTag = "Enemy";

    [Header("Object Pool")]
    [SerializeField] private int poolInitialSize = 5;
    [SerializeField] private int poolMaxSize = 10;

    private float shootTimer;
    private Transform currentTarget;
    private Vector2 aimDirection = Vector2.right;

    public Transform CurrentTarget => currentTarget;
    public Vector2 AimDirection => aimDirection;
    public bool HasTargetInRange => currentTarget != null;
    public bool IsReadyToShoot => shootTimer <= 0f;
    public float DetectionRange => detectionRange;
    public string TargetTag => targetTag;

    private void Start()
    {
        if (arrowPrefab != null && PoolManager.Instance != null)
            PoolManager.Instance.CreatePool(arrowPrefab, poolInitialSize, poolMaxSize);

        shootTimer = 0f;
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;
    }

    public void RefreshTarget()
    {
        currentTarget = FindClosestTarget();
    }

    public void AimAtTarget()
    {
        if (currentTarget == null)
            return;

        aimDirection = (currentTarget.position - launchPoint.position).normalized;
        ApplyLaunchRotation();
    }

    public void AimInDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0f)
            return;

        aimDirection = direction.normalized;
        ApplyLaunchRotation();
    }

    public bool TryShoot(Transform homingOrigin, Vector2? overrideDirection = null)
    {
        if (!IsReadyToShoot)
            return false;

        bool homing = false;
        Vector2 shootDirection;

        if (currentTarget != null)
        {
            shootDirection = (currentTarget.position - launchPoint.position).normalized;
            homing = true;
        }
        else if (overrideDirection.HasValue && overrideDirection.Value.sqrMagnitude > 0f)
        {
            shootDirection = overrideDirection.Value.normalized;
        }
        else
        {
            return false;
        }

        aimDirection = shootDirection;
        ApplyLaunchRotation();
        FireArrow(homing, homingOrigin ?? transform);
        shootTimer = shootCooldown;
        return true;
    }

    private void FireArrow(bool enableHoming, Transform homingOrigin)
    {
        if (arrowPrefab == null || launchPoint == null || PoolManager.Instance == null)
            return;

        GameObject arrow = PoolManager.Instance.GetObject(arrowPrefab);
        if (arrow == null)
            return;

        arrow.transform.position = launchPoint.position;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Arrow arrowComponent = arrow.GetComponent<Arrow>();
        if (arrowComponent != null)
            arrowComponent.Initialize(enableHoming, detectionRange, homingOrigin, targetTag);
    }

    private Transform FindClosestTarget()
    {
        if (string.IsNullOrEmpty(targetTag))
            return null;

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        if (targets.Length == 0)
            return null;

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject target in targets)
        {
            if (target == null)
                continue;

            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= detectionRange && dist < closestDist)
            {
                closestDist = dist;
                closest = target.transform;
            }
        }

        return closest;
    }

    private void ApplyLaunchRotation()
    {
        if (launchPoint == null)
            return;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        launchPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (currentTarget != null && launchPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(launchPoint.position, currentTarget.position);
        }
    }
}
