using System.Collections.Generic;
using UnityEngine;

public class StoreItemLogicManager : MonoBehaviour
{
    public static StoreItemLogicManager Instance;

    [SerializeField] private List<StoreItemLogicBase> itemLogics = new();

    private Dictionary<string, StoreItemLogicBase> logicById = new();

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
        logicById.Clear();

        foreach (StoreItemLogicBase logic in itemLogics)
        {
            if (logic == null || string.IsNullOrEmpty(logic.ItemId))
                continue;

            if (!logicById.ContainsKey(logic.ItemId))
            {
                logicById.Add(logic.ItemId, logic);
            }
            else
            {
                Debug.LogWarning($"StoreItemLogicManager: itemId duplicado '{logic.ItemId}'");
            }
        }
    }

    public StoreItemLogicBase GetLogicById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        logicById.TryGetValue(itemId, out StoreItemLogicBase logic);
        return logic;
    }
}