using UnityEngine;

public class BoxPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Box collected!");
        EventManager.Instance.MoneyCollected();
    }
}
