using UnityEngine;

public class BigLootPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Big Loot collected!");
        EventManager.Instance.MoneyCollected();
    }
}
