using UnityEngine;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform detectionOrigin;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float eatDistance = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string isSittingParam = "IsSitting";
    [SerializeField] private string isAngryParam = "IsAngry";
    [SerializeField] private string isEatingParam = "IsEating";
    [SerializeField] private string isBlockedParam = "IsBlocked";

    [Header("Eating")]
    [SerializeField] private Transform mouthPoint;

    private Transform currentTarget;
    private Transform currentMeatTarget;
    private bool isDistracted = false;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (detectionOrigin == null)
            detectionOrigin = transform;

        if (mouthPoint == null)
            mouthPoint = transform;

        SetSittingState(true);
        SetAngryState(false);
        SetEatingState(false);
        SetBlockedState(false);

        if (agent != null)
        {
            agent.speed = runSpeed;
            agent.isStopped = true;
        }
    }

    private void Update()
    {
        if (isDistracted)
        {
            HandleDistractedState();
            return;
        }

        DetectPlayer();

        if (currentTarget != null)
        {
            ChaseTarget();
        }
        else
        {
            StayIdle();
        }
    }

    private void DetectPlayer()
    {
        if (currentTarget != null)
            return;

        Collider[] hits = Physics.OverlapSphere(detectionOrigin.position, detectionRadius, playerMask);

        if (hits.Length > 0)
        {
            currentTarget = hits[0].transform;

            SetSittingState(false);
            SetAngryState(true);
            SetEatingState(false);
            SetBlockedState(false);

            if (agent != null)
            {
                agent.isStopped = false;
                agent.speed = runSpeed;
            }
        }
    }

    private void ChaseTarget()
    {
        if (agent == null || currentTarget == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (WorldStateManager.Instance != null &&
            WorldStateManager.Instance.IsPlayerInsideHouse)
        {
            agent.isStopped = true;
            UpdateAnimatorSpeed(0f);
            SetBlockedState(true);
            return;
        }

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(currentTarget.position);

        if (agent.pathPending)
        {
            SetBlockedState(false);
            return;
        }

        SetBlockedState(false);
        UpdateAnimatorSpeed(agent.velocity.magnitude);
    }

    private void HandleDistractedState()
    {
        if (agent == null)
            return;

        if (currentMeatTarget == null)
        {
            agent.isStopped = true;
            UpdateAnimatorSpeed(0f);
            return;
        }

        Vector3 targetPosition = currentMeatTarget.position;

        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(targetPosition);

        if (agent.pathPending)
            return;

        UpdateAnimatorSpeed(agent.velocity.magnitude);

        float distanceToMeat = Vector3.Distance(mouthPoint.position, currentMeatTarget.position);

        if (distanceToMeat <= eatDistance)
        {
            Debug.Log("Perro llegó a la carne. Distancia: " + distanceToMeat);
            StartEating();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0)
            return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCaughtScreen();
        }
        else
        {
            Debug.Log("El perro atrapó al jugador.");
        }
    }

    private void StayIdle()
    {
        if (agent != null)
            agent.isStopped = true;

        UpdateAnimatorSpeed(0f);
    }

    private void UpdateAnimatorSpeed(float value)
    {
        if (animator != null)
            animator.SetFloat(speedParam, value);
    }

    private void SetSittingState(bool value)
    {
        if (animator != null)
            animator.SetBool(isSittingParam, value);
    }

    private void SetAngryState(bool value)
    {
        if (animator != null)
            animator.SetBool(isAngryParam, value);
    }

    private void SetEatingState(bool value)
    {
        if (animator != null)
            animator.SetBool(isEatingParam, value);
    }

    private void SetBlockedState(bool value)
    {
        if (animator != null)
            animator.SetBool(isBlockedParam, value);
    }

    public void DistractWithMeat(Transform meatTarget)
    {
        if (meatTarget == null || agent == null)
            return;

        isDistracted = true;
        currentTarget = null;
        currentMeatTarget = meatTarget;

        SetAngryState(false);
        SetSittingState(false);
        SetEatingState(false);
        SetBlockedState(false);

        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(meatTarget.position);
    }

    public void StartEating()
    {
        if (agent != null)
            agent.isStopped = true;

        isDistracted = true;
        currentTarget = null;

        UpdateAnimatorSpeed(0f);
        SetBlockedState(false);
        SetAngryState(false);
        SetSittingState(false);
        SetEatingState(true);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionOrigin == null)
            detectionOrigin = transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionOrigin.position, detectionRadius);
    }
}