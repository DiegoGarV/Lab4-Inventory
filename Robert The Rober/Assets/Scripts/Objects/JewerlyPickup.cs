using UnityEngine;

public class JewerlyPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("Jewelry collected!");
        EventManager.Instance.MoneyCollected();
    }
}
