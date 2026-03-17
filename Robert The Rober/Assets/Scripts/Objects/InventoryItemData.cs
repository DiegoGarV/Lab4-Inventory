using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public Sprite itemIcon;
    public float sackValue;
    public int monetaryValue;

    public InventoryItemData(Pickup pickup)
    {
        itemName = pickup.ItemName;
        itemIcon = pickup.ItemIcon;
        sackValue = pickup.SackValue;
        monetaryValue = pickup.MonetaryValue;
    }
}