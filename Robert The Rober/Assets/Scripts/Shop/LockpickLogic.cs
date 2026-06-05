using UnityEngine;

public class LockpickLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("LockpickLogic: ganzúa disponible para usar.");
    }

    public override bool Use(RaycastHit hit)
    {
        DoorController door = hit.collider.GetComponentInParent<DoorController>();

        if (door == null)
            return false;

        if (door.CanOpenNormally())
        {
            door.Interact();
            return true;
        }

        if (PlayerProgressManager.Instance == null)
            return false;

        int lockpickCount = PlayerProgressManager.Instance.GetItemQuantity(ItemId);

        if (lockpickCount <= 0)
        {
            Debug.Log("No tienes ganzúas.");
            return true;
        }

        bool consumed = PlayerProgressManager.Instance.ConsumeItem(ItemId);

        if (!consumed)
        {
            Debug.Log("No se pudo consumir una ganzúa.");
            return true;
        }

        float successChance = GetSuccessChance(door.CurrentDoorLevel);
        float roll = Random.value;
        Debug.Log($"Probabilidad: {successChance * 100f}%");

        if (roll <= successChance)
        {
            door.UnlockDoor();
            door.OpenDoor();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLockpickSuccess();
            }

            return true;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLockpickFail();
        }

        return true;
    }

    public override bool CanUseOn(RaycastHit hit)
    {
        DoorController door = hit.collider.GetComponentInParent<DoorController>();
        return door != null && !door.CanOpenNormally();
    }

    private float GetSuccessChance(DoorController.DoorLevel level)
    {
        switch (level)
        {
            case DoorController.DoorLevel.CloseEasy:
                return 0.60f;

            case DoorController.DoorLevel.CloseMid:
                return 0.40f;

            case DoorController.DoorLevel.CloseHard:
                return 0.10f;

            case DoorController.DoorLevel.Open:
            default:
                return 1f;
        }
    }
}