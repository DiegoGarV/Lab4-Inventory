using System.Collections.Generic;
using UnityEngine;

public class StoreItemsManager : MonoBehaviour
{
    public static StoreItemsManager Instance;

    [SerializeField] private List<StoreItemDefinition> itemDefinitions = new();

    private Dictionary<string, StoreItemDefinition> definitionsById = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildLookup();
    }

    private void BuildLookup()
    {
        definitionsById.Clear();

        foreach (StoreItemDefinition def in itemDefinitions)
        {
            if (def == null || string.IsNullOrEmpty(def.itemId))
                continue;

            if (!definitionsById.ContainsKey(def.itemId))
            {
                definitionsById.Add(def.itemId, def);
            }
        }
    }

    public StoreItemDefinition GetDefinitionById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;

        definitionsById.TryGetValue(itemId, out StoreItemDefinition definition);
        return definition;
    }

    public void SyncStoreSceneItems()
    {
        if (PlayerProgressManager.Instance == null) return;

        StoreItemBase[] allStoreItems = FindObjectsByType<StoreItemBase>(FindObjectsSortMode.None);

        foreach (StoreItemBase item in allStoreItems)
        {
            if (item == null) continue;

            // Si no es consumible y ya fue comprado, ocultarlo/destruirlo en tienda
            if (!item.IsConsumable && PlayerProgressManager.Instance.HasItem(item.ItemId))
            {
                Destroy(item.gameObject);
            }
        }
    }
}