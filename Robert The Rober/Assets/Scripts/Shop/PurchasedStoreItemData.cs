using System;

[Serializable]
public class PurchasedStoreItemData
{
    public string itemId;
    public bool wasPurchased;
    public int quantity;

    public PurchasedStoreItemData(string itemId, bool wasPurchased, int quantity)
    {
        this.itemId = itemId;
        this.wasPurchased = wasPurchased;
        this.quantity = quantity;
    }
}