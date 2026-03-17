using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TMP_Text cashText;

    [Header("Sack UI")]
    [SerializeField] private Animator sackAnimator;
    [SerializeField] private string sackParameterName = "SackNormalized";
    [SerializeField] private float maxSackLoad = 10f;

    private int cashScore = 0;
    private float currentSackLoad = 0f;

    public float CurrentSackLoad => currentSackLoad;
    public float MaxSackLoad => maxSackLoad;

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
        UpdateCashText();
        UpdateSackBar();
    }

    private void HandlePickupCollected(Pickup pickup)
    {
        cashScore += pickup.MonetaryValue;

        if (!(pickup is MoneyPickup))
        {
            currentSackLoad += pickup.SackValue;
            currentSackLoad = Mathf.Clamp(currentSackLoad, 0f, maxSackLoad);
            UpdateSackBar();
        }

        UpdateCashText();
    }

    private void UpdateCashText()
    {
        if (cashText != null)
        {
            cashText.text = "Cash: $" + cashScore;
        }
    }

    private void UpdateSackBar()
    {
        if (sackAnimator == null) return;

        float normalizedLoad = Mathf.InverseLerp(0f, maxSackLoad, currentSackLoad);
        sackAnimator.SetFloat(sackParameterName, normalizedLoad);
    }

    public bool CanCollect(Pickup pickup)
    {
        if (pickup == null) return false;

        if (pickup is MoneyPickup)
            return true;

        return currentSackLoad + pickup.SackValue <= maxSackLoad;
    }
}