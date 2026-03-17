using UnityEngine;

public class MoneyPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Money collected!");
        EventManager.Instance.MoneyCollected();
    }
}
