using UnityEngine;
using System;

public abstract class Pickup : MonoBehaviour
{
    [Header("Inventory Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;

    [Header("Pickup Values")]
    [SerializeField] private float sackValue = 0f;
    [SerializeField] private int monetaryValue = 0;

    private EntityID entityID;

    public string PickupId => entityID != null ? entityID.ID : "";
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public float SackValue => sackValue;
    public int MonetaryValue => monetaryValue;

    public static event Action<Pickup> OnPickupCollected;

    protected virtual void Awake()
    {
        entityID = GetComponent<EntityID>();

        if (entityID == null)
        {
            Debug.LogError($"Pickup en {gameObject.name} no tiene EntityID.");
        }
    }

    public void Collect()
    {
        Use();
        OnPickupCollected?.Invoke(this);
        Destroy(gameObject);
    }

    protected abstract void Use();
}