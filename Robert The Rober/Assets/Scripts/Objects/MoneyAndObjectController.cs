using UnityEngine;
using System;

public class MoneyAndObjectsController : MonoBehaviour
{
    public static MoneyAndObjectsController Instance;

    [Header("Sack Settings")]
    [SerializeField] private float maxSackLoad = 150f;

    private int cashScore = 0;
    private int storedLootValue = 0;
    private float currentSackLoad = 0f;

    public int CashScore => cashScore;
    public int StoredLootValue => storedLootValue;
    public float CurrentSackLoad => currentSackLoad;
    public float MaxSackLoad => maxSackLoad;

    public event Action<int> OnCashChanged;
    public event Action<float, float> OnSackChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
        NotifyCurrentState();
    }

    private void HandlePickupCollected(Pickup pickup)
    {
        if (pickup == null) return;

        if (pickup is MoneyPickup)
        {
            cashScore += pickup.MonetaryValue;
            OnCashChanged?.Invoke(cashScore);
            return;
        }

        currentSackLoad += pickup.SackValue;
        currentSackLoad = Mathf.Clamp(currentSackLoad, 0f, maxSackLoad);
        storedLootValue += pickup.MonetaryValue;

        OnSackChanged?.Invoke(currentSackLoad, maxSackLoad);
    }

    public bool CanCollect(Pickup pickup)
    {
        if (pickup == null) return false;

        if (pickup is MoneyPickup)
            return true;

        return currentSackLoad + pickup.SackValue <= maxSackLoad;
    }

    public void ResetState()
    {
        cashScore = 0;
        storedLootValue = 0;
        currentSackLoad = 0f;
        NotifyCurrentState();
    }

    private void NotifyCurrentState()
    {
        OnCashChanged?.Invoke(cashScore);
        OnSackChanged?.Invoke(currentSackLoad, maxSackLoad);
    }
}
