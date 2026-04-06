using UnityEngine;
using System;

public abstract class Pickup : MonoBehaviour
{
    [Header("Persistence")]
    [SerializeField] private string pickupId;

    [Header("Inventory Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;

    [Header("Pickup Values")]
    [SerializeField] private float sackValue = 0f;
    [SerializeField] private int monetaryValue = 0;

    public string PickupId => pickupId;
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public float SackValue => sackValue;
    public int MonetaryValue => monetaryValue;

    public static event Action<Pickup> OnPickupCollected;

    public void Collect()
    {
        Use();
        OnPickupCollected?.Invoke(this);
        Destroy(gameObject);
    }

    protected abstract void Use();
}