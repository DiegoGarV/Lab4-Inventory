using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private bool isInventoryOpen = false;
    private readonly Dictionary<string, InventorySlotUI> slotByName = new();

    private void OnEnable()
    {
        Pickup.OnPickupCollected += HandlePickupCollected;
    }

    private void OnDisable()
    {
        Pickup.OnPickupCollected -= HandlePickupCollected;
    }

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
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
            Debug.LogError("InventoryUIManager: faltan referencias de inventorySlotPrefab o gridParent.");
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

    private void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
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

        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }
}