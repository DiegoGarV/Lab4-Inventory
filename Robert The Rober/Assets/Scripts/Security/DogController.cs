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

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string isSittingParam = "IsSitting";
    [SerializeField] private string isAngryParam = "IsAngry";
    [SerializeField] private string isEatingParam = "IsEating";
    [SerializeField] private string isBlockedParam = "IsBlocked";

    private Transform currentTarget;
    private bool isDistracted = false;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (detectionOrigin == null)
            detectionOrigin = transform;

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
            UpdateAnimatorSpeed(0f);
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

        UpdateAnimatorSpeed(0f);
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