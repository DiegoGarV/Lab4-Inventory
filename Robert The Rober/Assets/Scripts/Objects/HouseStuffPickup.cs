using UnityEngine;

public class HouseStuffPickup : Pickup
{
    protected override void Use()
    {
        Debug.Log("HouseStuff collected!");
        EventManager.Instance.MoneyCollected();
    }
}
