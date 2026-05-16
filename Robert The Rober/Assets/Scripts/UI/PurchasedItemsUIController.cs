using System.Collections.Generic;
using UnityEngine;

public class PurchasedItemsUIController : MonoBehaviour
{
    [Header("Purchased Items UI")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private readonly Dictionary<string, InventorySlotUI> slotById = new();

    public void RefreshPurchasedItemsUI()
    {
        ClearPurchasedItemsUI();

        if (PlayerProgressManager.Instance == null)
            return;

        List<PurchasedStoreItemData> purchasedItems = PlayerProgressManager.Instance.PurchasedItems;
        if (purchasedItems == null) return;

        foreach (PurchasedStoreItemData purchasedItem in purchasedItems)
        {
            // Luego aquí cambiaremos a catálogo global
            StoreItemBase storeItem = FindStoreItemById(purchasedItem.itemId);
            if (storeItem == null) continue;

            PurchasedItemUIData uiData = new PurchasedItemUIData(
                purchasedItem.itemId,
                storeItem.ItemName,
                storeItem.ItemIcon,
                purchasedItem.quantity,
                storeItem.IsConsumable
            );

            AddSlotToUI(uiData);
        }
    }

    private void AddSlotToUI(PurchasedItemUIData itemData)
    {
        if (inventorySlotPrefab == null || gridParent == null)
        {
            Debug.LogError("PurchasedItemsUIController: faltan referencias de inventorySlotPrefab o gridParent.");
            return;
        }

        GameObject slotGO = Instantiate(inventorySlotPrefab, gridParent);

        RectTransform rt = slotGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.one;
        }

        InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
        if (slotUI == null)
        {
            Debug.LogError("El prefab del slot no tiene InventorySlotUI.");
            return;
        }

        slotUI.SetPurchasedItemData(itemData);
        slotById[itemData.itemId] = slotUI;
    }

    public void ClearPurchasedItemsUI()
    {
        slotById.Clear();

        if (gridParent == null) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    private StoreItemBase FindStoreItemById(string itemId)
    {
        StoreItemBase[] allStoreItems = FindObjectsByType<StoreItemBase>(FindObjectsSortMode.None);

        foreach (StoreItemBase item in allStoreItems)
        {
            if (item.ItemId == itemId)
                return item;
        }

        return null;
    }
}