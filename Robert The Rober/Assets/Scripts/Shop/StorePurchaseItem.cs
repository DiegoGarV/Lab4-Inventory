using UnityEngine;

public class StorePurchaseItem : MonoBehaviour
{
    [Header("Item Definition")]
    [SerializeField] private StoreItemDefinition itemDefinition;

    public StoreItemDefinition ItemDefinition => itemDefinition;

    public string ItemId => itemDefinition != null ? itemDefinition.itemId : "";
    public string ItemName => itemDefinition != null ? itemDefinition.itemName : "";
    public Sprite ItemIcon => itemDefinition != null ? itemDefinition.itemIcon : null;
    public int ItemPrice => itemDefinition != null ? itemDefinition.itemPrice : 0;
    public bool DestroyOnPurchase => itemDefinition != null && itemDefinition.destroyOnPurchase;
    public bool IsConsumable => itemDefinition != null && itemDefinition.isConsumable;
    public StoreItemEffectType EffectType => itemDefinition != null ? itemDefinition.effectType : StoreItemEffectType.None;
    public string EffectDescription => itemDefinition != null ? itemDefinition.effectDescription : "";

    private void OnValidate()
    {
        if (itemDefinition == null)
        {
            Debug.LogWarning($"StorePurchaseItem en '{gameObject.name}' no tiene StoreItemDefinition asignado.", this);
        }
    }

    public virtual bool CanBePurchased(int currentMoney)
    {
        if (itemDefinition == null) return false;
        if (currentMoney < ItemPrice) return false;
        if (PlayerProgressManager.Instance == null) return false;

        if (!IsConsumable && PlayerProgressManager.Instance.HasItem(ItemId))
            return false;

        return true;
    }

    public virtual void Purchase()
    {
        if (DestroyOnPurchase)
        {
            Destroy(gameObject);
        }
    }
}