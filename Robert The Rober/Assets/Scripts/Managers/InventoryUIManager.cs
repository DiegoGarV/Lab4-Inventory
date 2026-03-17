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
        InventoryItemData newItem = new InventoryItemData(pickup);

        if (slotByName.TryGetValue(newItem.itemName, out InventorySlotUI existingSlot))
        {
            existingSlot.AddOne();
            return;
        }

        AddSlotToUI(newItem);
    }

    private void AddSlotToUI(InventoryItemData itemData)
    {
        if (gridParent == null)
        {
            Debug.LogError("gridParent no está asignado");
            return;
        }

        if (inventorySlotPrefab == null)
        {
            Debug.LogError("inventorySlotPrefab no está asignado");
            return;
        }

        GameObject slotGO = Instantiate(inventorySlotPrefab, gridParent);

        RectTransform rt = slotGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.one;
        }

        InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
        if (slotUI != null)
        {
            slotUI.SetData(itemData);
            slotByName[itemData.itemName] = slotUI;
        }
        else
        {
            Debug.LogError("El prefab no tiene InventorySlotUI");
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