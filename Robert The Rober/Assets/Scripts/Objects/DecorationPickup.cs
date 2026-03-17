using UnityEngine;

public class DecorationPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Decoration collected!");
        EventManager.Instance.MoneyCollected();
    }
}
