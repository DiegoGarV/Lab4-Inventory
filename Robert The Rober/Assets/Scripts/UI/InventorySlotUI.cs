using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;

    private string itemName;
    private int quantity = 0;

    public string ItemName => itemName;

    public void SetData(InventoryItemData data)
    {
        itemName = data.itemName;
        quantity = 1;

        Debug.Log("Nombre item: " + data.itemName);
        Debug.Log("Sprite item: " + (data.itemIcon != null ? data.itemIcon.name : "NULL"));

        if (iconImage != null)
        {
            iconImage.sprite = data.itemIcon;
            iconImage.enabled = data.itemIcon != null;
            iconImage.preserveAspect = true;
        }

        if (nameText != null)
        {
            nameText.text = data.itemName;
        }

        UpdateQuantityText();
    }

    public void AddOne()
    {
        quantity++;
        UpdateQuantityText();
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            quantityText.text = "x" + quantity;
        }
    }
}