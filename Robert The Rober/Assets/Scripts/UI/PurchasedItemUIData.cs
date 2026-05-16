using UnityEngine;

[System.Serializable]
public class PurchasedItemUIData
{
    public string itemId;
    public string itemName;
    public Sprite itemIcon;
    public int quantity;
    public bool isConsumable;

    public PurchasedItemUIData(string itemId, string itemName, Sprite itemIcon, int quantity, bool isConsumable)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.quantity = quantity;
        this.isConsumable = isConsumable;
    }
}