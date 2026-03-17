using UnityEngine;

public class ArtefactsPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Artefact collected!");
        EventManager.Instance.MoneyCollected();
    }
}
