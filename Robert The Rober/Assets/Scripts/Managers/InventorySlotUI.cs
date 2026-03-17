using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    public void SetData(InventoryItemData data)
    {
        if (iconImage != null)
        {
            iconImage.sprite = data.itemIcon;
            iconImage.enabled = data.itemIcon != null;
        }

        if (nameText != null)
        {
            nameText.text = data.itemName;
        }
    }
}