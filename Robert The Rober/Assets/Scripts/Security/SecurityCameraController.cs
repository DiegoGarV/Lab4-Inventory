using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float viewDistance = 8f;
    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private LayerMask playerMask;

    [Header("References")]
    [SerializeField] private Transform viewOrigin;
    [SerializeField] private GameObject visionVisual;

    private bool isPowered = true;

    public bool IsPowered => isPowered;

    private void Start()
    {
        if (viewOrigin == null)
            viewOrigin = transform;

        UpdateVisionVisual();
    }

    private void Update()
    {
        if (!isPowered)
            return;

        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(viewOrigin.position, viewDistance, playerMask);

        foreach (Collider hit in hits)
        {
            Transform target = hit.transform;

            Vector3 directionToTarget = (target.position - viewOrigin.position).normalized;
            float angle = Vector3.Angle(viewOrigin.forward, directionToTarget);

            if (angle > viewAngle * 0.5f)
                continue;

            float distanceToTarget = Vector3.Distance(viewOrigin.position, target.position);

            if (!Physics.Raycast(viewOrigin.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                Debug.Log($"Cámara {name}: jugador detectado.");
                return;
            }
        }
    }

    public void SetPowered(bool value)
    {
        isPowered = value;
        UpdateVisionVisual();
    }

    private void UpdateVisionVisual()
    {
        if (visionVisual != null)
            visionVisual.SetActive(isPowered);
    }

    private void OnDrawGizmosSelected()
    {
        if (viewOrigin == null)
            viewOrigin = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(viewOrigin.position, viewDistance);

        Vector3 leftDir = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * viewOrigin.forward;
        Vector3 rightDir = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * viewOrigin.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(viewOrigin.position, leftDir * viewDistance);
        Gizmos.DrawRay(viewOrigin.position, rightDir * viewDistance);
        Gizmos.DrawRay(viewOrigin.position, viewOrigin.forward * viewDistance);
    }

    public float ViewDistance => viewDistance;
    public float ViewAngle => viewAngle;
    public Transform ViewOrigin => viewOrigin;
}