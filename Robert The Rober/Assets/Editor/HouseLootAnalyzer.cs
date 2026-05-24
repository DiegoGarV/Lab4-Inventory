using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class HouseLootAnalyzer
{
    private class LootCandidate
    {
        public Pickup pickup;
        public string objectName;
        public string entityId;
        public int value;
        public int weight;
    }

    private class KnapsackResult
    {
        public int totalValue;
        public List<LootCandidate> selectedItems = new();
    }

    [MenuItem("Tools/Loot Analyzer/Analyze Current House Prefab")]
    public static void AnalyzeCurrentHousePrefab()
    {
        GameObject root = GetAnalysisRoot();

        if (root == null)
        {
            EditorUtility.DisplayDialog(
                "Loot Analyzer",
                "No encontré un prefab abierto en Prefab Mode ni un GameObject seleccionado para analizar.",
                "OK"
            );
            return;
        }

        Pickup[] pickups = root.GetComponentsInChildren<Pickup>(true);

        if (pickups == null || pickups.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Loot Analyzer",
                $"No se encontraron objetos con Pickup dentro de: {root.name}",
                "OK"
            );
            return;
        }

        int totalMoneyAll = 0;
        int totalMoneyOnlyMoneyPickup = 0;
        List<LootCandidate> nonMoneyCandidates = new();

        foreach (Pickup pickup in pickups)
        {
            if (pickup == null) continue;

            totalMoneyAll += pickup.MonetaryValue;

            if (pickup is MoneyPickup)
            {
                totalMoneyOnlyMoneyPickup += pickup.MonetaryValue;
                continue;
            }

            LootCandidate candidate = new LootCandidate
            {
                pickup = pickup,
                objectName = pickup.gameObject.name,
                entityId = string.IsNullOrEmpty(pickup.PickupId) ? "SIN_PICKUP_ID" : pickup.PickupId,
                value = pickup.MonetaryValue,
                weight = Mathf.Max(0, Mathf.RoundToInt(pickup.SackValue))
            };

            nonMoneyCandidates.Add(candidate);
        }

        KnapsackResult result150 = SolveKnapsack(nonMoneyCandidates, 150);
        KnapsackResult result300 = SolveKnapsack(nonMoneyCandidates, 300);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("===== HOUSE LOOT ANALYZER =====");
        sb.AppendLine($"Root analizado: {root.name}");
        sb.AppendLine($"Cantidad total de pickups encontrados: {pickups.Length}");
        sb.AppendLine();

        sb.AppendLine($"Dinero total en la casa: ${totalMoneyAll}");
        sb.AppendLine($"Dinero solo de MoneyPickup: ${totalMoneyOnlyMoneyPickup}");
        sb.AppendLine();

        sb.AppendLine("----- Mejor robo con saco de 150 -----");
        sb.AppendLine($"Valor máximo: ${result150.totalValue}");
        AppendSelectedItems(sb, result150.selectedItems);
        sb.AppendLine();

        sb.AppendLine("----- Mejor robo con saco de 300 -----");
        sb.AppendLine($"Valor máximo: ${result300.totalValue}");
        AppendSelectedItems(sb, result300.selectedItems);
        sb.AppendLine();

        Debug.Log(sb.ToString());

        EditorGUIUtility.systemCopyBuffer = sb.ToString();

        EditorUtility.DisplayDialog(
            "Loot Analyzer",
            "Análisis completado.\n\nSe imprimió en la consola y también se copió al portapapeles.",
            "OK"
        );
    }

    private static GameObject GetAnalysisRoot()
    {
        // Primero intenta usar el prefab abierto en Prefab Mode
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null && prefabStage.prefabContentsRoot != null)
        {
            return prefabStage.prefabContentsRoot;
        }

        // Si no hay Prefab Mode, usa el objeto seleccionado
        if (Selection.activeGameObject != null)
        {
            return Selection.activeGameObject;
        }

        return null;
    }

    private static void AppendSelectedItems(StringBuilder sb, List<LootCandidate> items)
    {
        if (items == null || items.Count == 0)
        {
            sb.AppendLine("No se seleccionaron objetos.");
            return;
        }

        int totalWeight = 0;

        foreach (LootCandidate item in items)
        {
            totalWeight += item.weight;
        }

        sb.AppendLine($"Cantidad de objetos: {items.Count}");
        sb.AppendLine($"Peso total usado: {totalWeight}");
        sb.AppendLine("Objetos:");

        foreach (LootCandidate item in items)
        {
            sb.AppendLine(
                $"- {item.objectName} | EntityID: {item.entityId} | Valor: ${item.value} | Peso: {item.weight}"
            );
        }
    }

    private static KnapsackResult SolveKnapsack(List<LootCandidate> items, int capacity)
    {
        int n = items.Count;
        int[,] dp = new int[n + 1, capacity + 1];

        for (int i = 1; i <= n; i++)
        {
            int weight = items[i - 1].weight;
            int value = items[i - 1].value;

            for (int w = 0; w <= capacity; w++)
            {
                dp[i, w] = dp[i - 1, w];

                if (weight <= w)
                {
                    int includeValue = dp[i - 1, w - weight] + value;
                    if (includeValue > dp[i, w])
                    {
                        dp[i, w] = includeValue;
                    }
                }
            }
        }

        List<LootCandidate> selected = new List<LootCandidate>();
        int remainingCapacity = capacity;

        for (int i = n; i > 0; i--)
        {
            if (dp[i, remainingCapacity] != dp[i - 1, remainingCapacity])
            {
                LootCandidate chosen = items[i - 1];
                selected.Add(chosen);
                remainingCapacity -= chosen.weight;
            }
        }

        selected.Reverse();

        return new KnapsackResult
        {
            totalValue = dp[n, capacity],
            selectedItems = selected
        };
    }
}