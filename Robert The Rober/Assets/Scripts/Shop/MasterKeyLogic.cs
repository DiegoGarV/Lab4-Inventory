using UnityEngine;

public class MasterKeyLogic : StoreItemLogicBase
{
    public override void ApplyLevelStartEffect()
    {
        Debug.Log("MasterKeyLogic: llave maestra disponible para usar.");
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

        door.UnlockDoor();
        door.OpenDoor();

        Debug.Log("MasterKeyLogic: la puerta se abrió con la llave maestra.");
        return true;
    }

    public override bool CanUseOn(RaycastHit hit)
    {
        DoorController door = hit.collider.GetComponentInParent<DoorController>();
        return door != null && door.CanOpenNormally();
    }
}