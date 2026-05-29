using UnityEngine;

public class LockpickItem : StoreItemBase
{
    public const string LockpickItemId = "lockpick";

    protected override void ApplyEffect()
    {
        Debug.Log("La ganzúa se usa sobre una puerta desde ActionController.");
    }

    public override void ApplyLevelStartEffect()
    {
        Debug.Log("Ganzúa comprada y disponible para usar.");
    }

    protected override void OnConsumedCompletely()
    {
        Debug.Log("Ya no quedan ganzúas.");
    }

    public static bool TryUseOnDoor(DoorController door)
    {
        if (door == null)
            return false;

        if (door.CanOpenNormally())
        {
            door.Interact();
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
        return false;
    }

    private static float GetSuccessChance(DoorController.DoorLevel level)
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