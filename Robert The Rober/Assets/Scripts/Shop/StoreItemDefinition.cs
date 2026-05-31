using UnityEngine;

[CreateAssetMenu(fileName = "StoreItemDefinition", menuName = "Store/Item Definition")]
public class StoreItemDefinition : ScriptableObject
{
    [Header("Core Info")]
    public string itemId;
    public string itemName;
    public Sprite itemIcon;
    public int itemPrice;

    [Header("Purchase Rules")]
    public bool isConsumable;
    public bool destroyOnPurchase;

    [Header("Effect Info")]
    public StoreItemEffectType effectType;
    [TextArea]
    public string effectDescription;

    public virtual void ApplyLevelStartEffect()
    {
    }
}