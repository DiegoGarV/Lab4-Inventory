using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager Instance;

    [Header("Progress")]
    [SerializeField] private int currentCurrency = 0;
    [SerializeField] private string currentLevelName = "";

    [Header("Purchased Store Items")]
    [SerializeField] private List<PurchasedStoreItemData> purchasedItems = new();

    public int CurrentCurrency => currentCurrency;
    public string CurrentLevelName => currentLevelName;
    public List<PurchasedStoreItemData> PurchasedItems => purchasedItems;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetCurrentLevel(scene.name);

        if (IsHeistScene(scene.name))
        {
            ApplyLevelStartItemEffects();
        }
    }

    private bool IsHeistScene(string sceneName)
    {
        return sceneName == "MorningLevel" || sceneName == "AfternoonLevel";
    }

    private void ApplyLevelStartItemEffects()
    {
        if (MoneyAndObjectsController.Instance != null)
        {
            MoneyAndObjectsController.Instance.ResetSackCapacity();
        }
        
        if (StoreItemLogicManager.Instance == null)
        {
            Debug.LogWarning("PlayerProgressManager: StoreItemLogicManager.Instance es null.");
            return;
        }

        foreach (PurchasedStoreItemData purchasedItem in purchasedItems)
        {
            if (purchasedItem == null || !purchasedItem.wasPurchased)
                continue;

            StoreItemLogicBase itemLogic = StoreItemLogicManager.Instance.GetLogicById(purchasedItem.itemId);

            if (itemLogic == null)
            {
                Debug.LogWarning($"PlayerProgressManager: no se encontró lógica para itemId {purchasedItem.itemId}");
                continue;
            }

            itemLogic.ApplyLevelStartEffect();
        }
    }

    public void SetCurrency(int value)
    {
        currentCurrency = Mathf.Max(0, value);
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        if (currentCurrency < 0)
            currentCurrency = 0;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount < 0) return false;
        if (currentCurrency < amount) return false;

        currentCurrency -= amount;
        return true;
    }

    public void SetCurrentLevel(string levelName)
    {
        currentLevelName = levelName;
    }

    public bool HasItem(string itemId)
    {
        PurchasedStoreItemData item = purchasedItems.Find(x => x.itemId == itemId);
        return item != null && item.wasPurchased;
    }

    public int GetItemQuantity(string itemId)
    {
        PurchasedStoreItemData item = purchasedItems.Find(x => x.itemId == itemId);
        return item != null ? item.quantity : 0;
    }

    public void RegisterPurchase(StorePurchaseItem item)
    {
        if (item == null) return;

        PurchasedStoreItemData existingItem = purchasedItems.Find(x => x.itemId == item.ItemId);

        if (existingItem != null)
        {
            existingItem.wasPurchased = true;
            existingItem.quantity += 1;
        }
        else
        {
            purchasedItems.Add(new PurchasedStoreItemData(item.ItemId, true, 1));
        }
    }

    public bool ConsumeItem(string itemId, int amount = 1)
    {
        if (amount <= 0) return false;

        PurchasedStoreItemData item = purchasedItems.Find(x => x.itemId == itemId);
        if (item == null) return false;
        if (item.quantity < amount) return false;

        item.quantity -= amount;

        if (item.quantity <= 0)
            item.quantity = 0;

        return true;
    }

    public void ResetProgress()
    {
        currentCurrency = 0;
        currentLevelName = "";
        purchasedItems.Clear();
    }
}