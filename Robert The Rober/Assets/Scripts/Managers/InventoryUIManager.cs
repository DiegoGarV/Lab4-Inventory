using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private readonly List<InventoryItemData> inventoryItems = new();
    private bool isInventoryOpen = false;

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
        if (pickup is MoneyPickup) return;

        InventoryItemData newItem = new InventoryItemData(pickup);
        inventoryItems.Add(newItem);
        AddSlotToUI(newItem);
    }

    private void AddSlotToUI(InventoryItemData itemData)
    {
        if (gridParent == null || inventorySlotPrefab == null) return;

        GameObject slotGO = Instantiate(inventorySlotPrefab, gridParent);
        InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

        if (slotUI != null)
        {
            slotUI.SetData(itemData);
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }
}