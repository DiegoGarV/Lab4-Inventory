using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private readonly Dictionary<string, InventorySlotUI> slotByName = new();

    private void OnEnable()
    {
        Pickup.OnPickupCollected += HandlePickupCollected;
    }

    private void OnDisable()
    {
        Pickup.OnPickupCollected -= HandlePickupCollected;
    }

    private void HandlePickupCollected(Pickup pickup)
    {
        if (pickup == null) return;
        if (pickup is MoneyPickup) return;

        InventoryItemData itemData = new InventoryItemData(pickup);

        if (slotByName.TryGetValue(itemData.itemName, out InventorySlotUI existingSlot))
        {
            existingSlot.AddOne();
            return;
        }

        AddSlotToUI(itemData);
    }

    private void AddSlotToUI(InventoryItemData itemData)
    {
        if (inventorySlotPrefab == null || gridParent == null)
        {
            Debug.LogError("InventoryUIController: faltan referencias de inventorySlotPrefab o gridParent.");
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

        slotUI.SetData(itemData);
        slotByName[itemData.itemName] = slotUI;
    }

    public void RefreshStolenItemsUI()
    {
        // Si tu inventario ya se llena en vivo por eventos,
        // este método puede quedarse vacío por ahora.
        // Lo dejamos para mantener la misma estructura que PurchasedItemsUIController.
    }

    public void LoadInventoryFromSave(List<string> collectedPickupIds)
    {
        if (collectedPickupIds == null) return;

        ClearInventoryUI();

        Pickup[] allPickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None);

        foreach (string id in collectedPickupIds)
        {
            foreach (Pickup pickup in allPickups)
            {
                if (pickup.PickupId == id)
                {
                    if (pickup is MoneyPickup) break;

                    InventoryItemData itemData = new InventoryItemData(pickup);

                    if (slotByName.TryGetValue(itemData.itemName, out InventorySlotUI existingSlot))
                    {
                        existingSlot.AddOne();
                    }
                    else
                    {
                        AddSlotToUI(itemData);
                    }

                    break;
                }
            }
        }
    }

    public void ClearInventoryUI()
    {
        slotByName.Clear();

        if (gridParent == null) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }
}