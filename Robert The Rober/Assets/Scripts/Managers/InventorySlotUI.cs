using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;

    public void SetData(InventoryItemData data)
    {
        Debug.Log("SetData llamado para: " + data.itemName);

        if (iconImage != null)
        {
            iconImage.sprite = data.itemIcon;
            iconImage.enabled = data.itemIcon != null;
            iconImage.preserveAspect = true;

            Debug.Log("Icono asignado: " + (data.itemIcon != null ? data.itemIcon.name : "NULL"));
        }

        if (nameText != null)
        {
            nameText.text = data.itemName;
            Debug.Log("Nombre asignado: " + data.itemName);
        }

        if (quantityText != null)
        {
            quantityText.text = "x1";
        }
    }
}