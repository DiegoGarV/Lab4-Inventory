using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    public enum DoorLevel
    {
        Open,
        CloseEasy,
        CloseMid,
        CloseHard
    }

    [Header("Door Settings")]
    [SerializeField] private DoorLevel doorLevel = DoorLevel.Open;
    [SerializeField] private float closedYRotation = 0f;
    [SerializeField] private float openYRotation = 110f;

    [Header("Current State")]
    [SerializeField] private bool isOpen = false;

    [Header("Navigation")]
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    private bool wasOpened = false;

    public DoorLevel CurrentDoorLevel => doorLevel;
    public bool IsOpen => isOpen;
    public bool WasOpened => wasOpened;

    private void Start()
    {
        if (navMeshObstacle == null)
            navMeshObstacle = GetComponent<NavMeshObstacle>();

        ApplyRotationInstant();
        UpdateNavigationState();
    }

    public bool CanOpenNormally()
    {
        return wasOpened || doorLevel == DoorLevel.Open;
    }

    public void Interact()
    {
        if (!CanOpenNormally())
        {
            Debug.Log("La puerta está cerrada. Necesitas una herramienta para abrirla.");
            return;
        }

        ToggleDoor();
    }

    public void UnlockDoor()
    {
        wasOpened = true;
        doorLevel = DoorLevel.Open;
    }

    public void OpenDoor()
    {
        isOpen = true;
        SetYRotation(openYRotation);
        UpdateNavigationState();
    }

    public void CloseDoor()
    {
        isOpen = false;
        SetYRotation(closedYRotation);
        UpdateNavigationState();
    }

    public void ToggleDoor()
    {
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    private void ApplyRotationInstant()
    {
        float targetY = isOpen ? openYRotation : closedYRotation;
        SetYRotation(targetY);
    }

    private void SetYRotation(float yRotation)
    {
        Vector3 currentEuler = transform.localEulerAngles;
        currentEuler.y = yRotation;
        transform.localEulerAngles = currentEuler;
    }

    private void UpdateNavigationState()
    {
        if (navMeshObstacle == null)
            return;

        navMeshObstacle.enabled = !isOpen;
    }

    public void SetDoorLevel(DoorLevel newLevel)
    {
        doorLevel = newLevel;
    }

    public void ForceClosed()
    {
        isOpen = false;
        SetYRotation(closedYRotation);
        UpdateNavigationState();
    }

    public void SetOpenState(bool value)
    {
        isOpen = value;

        if (isOpen)
            SetYRotation(openYRotation);
        else
            SetYRotation(closedYRotation);

        UpdateNavigationState();
    }

    public void UpgradeSecurityLevel()
    {
        switch (doorLevel)
        {
            case DoorLevel.Open:
                doorLevel = DoorLevel.CloseEasy;
                break;

            case DoorLevel.CloseEasy:
                doorLevel = DoorLevel.CloseMid;
                break;

            case DoorLevel.CloseMid:
                doorLevel = DoorLevel.CloseHard;
                break;

            case DoorLevel.CloseHard:
                break;
        }
    }
}