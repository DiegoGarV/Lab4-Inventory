using UnityEngine;

public enum StoreItemEffectType
{
    None,
    OpenDoors,
    IncreaseSackSize,
    DistractDog,
    RevealItemPrices,
    BreakLocks,
    DisableCameras
}

public abstract class StoreItemBase : MonoBehaviour
{
    [Header("Store Item Info")]
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private int itemPrice = 0;

    [Header("Purchase Rules")]
    [SerializeField] private bool destroyOnPurchase = true;
    [SerializeField] private bool isConsumable = false;

    [Header("Purchase State")]
    [SerializeField] private bool wasPurchased = false;
    [SerializeField] private int quantity = 0;

    [Header("Special Effect")]
    [SerializeField] private StoreItemEffectType effectType = StoreItemEffectType.None;
    [TextArea]
    [SerializeField] private string effectDescription;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public int ItemPrice => itemPrice;
    public bool DestroyOnPurchase => destroyOnPurchase;
    public bool IsConsumable => isConsumable;
    public bool WasPurchased => wasPurchased;
    public int Quantity => quantity;
    public StoreItemEffectType EffectType => effectType;
    public string EffectDescription => effectDescription;

    public virtual bool CanBePurchased(int currentMoney)
    {
        if (currentMoney < itemPrice)
            return false;

        // Si no es consumible, solo puede comprarse una vez
        if (!isConsumable && wasPurchased)
            return false;

        return true;
    }

    public virtual void Purchase()
    {
        wasPurchased = true;
        quantity++;

        OnPurchaseRegistered();

        if (destroyOnPurchase)
        {
            Destroy(gameObject);
        }
    }

    public virtual bool CanBeUsed()
    {
        if (isConsumable)
            return quantity > 0;

        return wasPurchased;
    }

    public virtual void Use()
    {
        if (!CanBeUsed())
            return;

        ApplyEffect();

        if (isConsumable)
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
        return isConsumable && quantity <= 0;
    }

    // Para registrar compras, guardado, inventario, etc.
    protected virtual void OnPurchaseRegistered()
    {
    }

    // Para cuando un consumible se agota por completo
    protected virtual void OnConsumedCompletely()
    {
    }

    // Para efectos que deben aplicarse al inicio de cada nivel
    public virtual void ApplyLevelStartEffect()
    {
    }

    // El efecto real se aplica después, no en la tienda
    protected abstract void ApplyEffect();
}