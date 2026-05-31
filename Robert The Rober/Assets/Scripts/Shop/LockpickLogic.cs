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

        if (roll <= successChance)
        {
            door.UnlockDoor();
            door.OpenDoor();
            Debug.Log($"Ganzúa exitosa. Probabilidad: {successChance * 100f}%");
            return true;
        }

        Debug.Log($"La ganzúa falló. Probabilidad: {successChance * 100f}%");
        return true;
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