using UnityEngine;

public abstract class StoreItemBase : MonoBehaviour
{
    [Header("Item Definition")]
    [SerializeField] private StoreItemDefinition itemDefinition;

    [Header("Purchase State")]
    [SerializeField] private bool wasPurchased = false;
    [SerializeField] private int quantity = 0;

    public StoreItemDefinition ItemDefinition => itemDefinition;

    public string ItemId => itemDefinition != null ? itemDefinition.itemId : "";
    public string ItemName => itemDefinition != null ? itemDefinition.itemName : "";
    public Sprite ItemIcon => itemDefinition != null ? itemDefinition.itemIcon : null;
    public int ItemPrice => itemDefinition != null ? itemDefinition.itemPrice : 0;
    public bool DestroyOnPurchase => itemDefinition != null && itemDefinition.destroyOnPurchase;
    public bool IsConsumable => itemDefinition != null && itemDefinition.isConsumable;
    public StoreItemEffectType EffectType => itemDefinition != null ? itemDefinition.effectType : StoreItemEffectType.None;
    public string EffectDescription => itemDefinition != null ? itemDefinition.effectDescription : "";

    public bool WasPurchased => wasPurchased;
    public int Quantity => quantity;

    public virtual bool CanBePurchased(int currentMoney)
    {
        if (itemDefinition == null) return false;
        if (currentMoney < ItemPrice) return false;

        if (!IsConsumable && wasPurchased)
            return false;

        return true;
    }

    public virtual void Purchase()
    {
        wasPurchased = true;
        quantity++;

        OnPurchaseRegistered();

        if (DestroyOnPurchase)
        {
            Destroy(gameObject);
        }
    }

    public virtual bool CanBeUsed()
    {
        if (IsConsumable)
            return quantity > 0;

        return wasPurchased;
    }

    public virtual void Use()
    {
        if (!CanBeUsed())
            return;

        ApplyEffect();

        if (IsConsumable)
        {
            quantity--;

            if (quantity < 0)
                quantity = 0;

            if (ShouldBeRemovedAfterUse())
            {
                OnConsumedCompletely();
            }
        }
    }

    protected virtual bool ShouldBeRemovedAfterUse()
    {
        return IsConsumable && quantity <= 0;
    }

    protected virtual void OnPurchaseRegistered()
    {
    }

    protected virtual void OnConsumedCompletely()
    {
    }

    protected virtual void ApplyEffect()
    {
    }

    public virtual void ApplyLevelStartEffect()
    {
    }
}