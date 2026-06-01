using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class HouseLootAnalyzer
{
    private const int CameraIgnoreAboveValue = 30000;
    private const float CameraActivationPercent = 0.60f;

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

        // Base para cámaras: mejor robo con saco 150 ignorando valores > 30000
        List<LootCandidate> cameraRelevantCandidates = FilterByMaxValue(nonMoneyCandidates, CameraIgnoreAboveValue);
        KnapsackResult cameraBase150 = SolveKnapsack(cameraRelevantCandidates, 150);

        int safeCameraMaxValue = Mathf.CeilToInt(cameraBase150.totalValue * CameraActivationPercent) - 1;
        if (safeCameraMaxValue < 0)
            safeCameraMaxValue = 0;

        // Mejor loot para NO activar cámaras
        KnapsackResult safeCameras150 = SolveKnapsackWithMaxValue(cameraRelevantCandidates, 150, safeCameraMaxValue);
        KnapsackResult safeCameras300 = SolveKnapsackWithMaxValue(cameraRelevantCandidates, 300, safeCameraMaxValue);

        // Mejor loot para NO activar perro
        List<LootCandidate> dogSafeCandidates = FilterByMaxWeightExclusive(nonMoneyCandidates, 50);
        KnapsackResult safeDog150 = SolveKnapsack(dogSafeCandidates, 150);
        KnapsackResult safeDog300 = SolveKnapsack(dogSafeCandidates, 300);

        // Mejor loot para NO activar ambos
        List<LootCandidate> safeBothCandidates = FilterByMaxWeightExclusive(cameraRelevantCandidates, 50);
        KnapsackResult safeBoth150 = SolveKnapsackWithMaxValue(safeBothCandidates, 150, safeCameraMaxValue);
        KnapsackResult safeBoth300 = SolveKnapsackWithMaxValue(safeBothCandidates, 300, safeCameraMaxValue);

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

        sb.AppendLine("===== REGLA DE CÁMARAS =====");
        sb.AppendLine($"Se ignoran objetos con valor mayor a ${CameraIgnoreAboveValue}");
        sb.AppendLine($"Mejor robo base para cámaras (saco 150): ${cameraBase150.totalValue}");
        sb.AppendLine($"Umbral para activar cámaras (60%): ${Mathf.CeilToInt(cameraBase150.totalValue * CameraActivationPercent)}");
        sb.AppendLine($"Máximo seguro para NO activar cámaras: ${safeCameraMaxValue}");
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar cámaras (saco 150) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeCameras150.totalValue}");
        AppendSelectedItems(sb, safeCameras150.selectedItems);
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar cámaras (saco 300) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeCameras300.totalValue}");
        AppendSelectedItems(sb, safeCameras300.selectedItems);
        sb.AppendLine();

        sb.AppendLine("===== REGLA DEL PERRO =====");
        sb.AppendLine("El perro se activa si robas al menos un objeto con peso >= 50");
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar perro (saco 150) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeDog150.totalValue}");
        AppendSelectedItems(sb, safeDog150.selectedItems);
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar perro (saco 300) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeDog300.totalValue}");
        AppendSelectedItems(sb, safeDog300.selectedItems);
        sb.AppendLine();

        sb.AppendLine("===== REGLA COMBINADA =====");
        sb.AppendLine("Evitar cámaras + evitar perro");
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar cámaras NI perro (saco 150) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeBoth150.totalValue}");
        AppendSelectedItems(sb, safeBoth150.selectedItems);
        sb.AppendLine();

        sb.AppendLine("----- Mejor loot para NO activar cámaras NI perro (saco 300) -----");
        sb.AppendLine($"Valor máximo seguro: ${safeBoth300.totalValue}");
        AppendSelectedItems(sb, safeBoth300.selectedItems);
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
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null && prefabStage.prefabContentsRoot != null)
        {
            return prefabStage.prefabContentsRoot;
        }

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

    private static List<LootCandidate> FilterByMaxValue(List<LootCandidate> items, int maxAllowedValue)
    {
        List<LootCandidate> filtered = new();

        foreach (LootCandidate item in items)
        {
            if (item.value <= maxAllowedValue)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }

    private static List<LootCandidate> FilterByMaxWeightExclusive(List<LootCandidate> items, int forbiddenWeightOrMore)
    {
        List<LootCandidate> filtered = new();

        foreach (LootCandidate item in items)
        {
            if (item.weight < forbiddenWeightOrMore)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }

    private static KnapsackResult SolveKnapsack(List<LootCandidate> items, int capacity)
    {
        return SolveKnapsackWithMaxValue(items, capacity, int.MaxValue);
    }

    private static KnapsackResult SolveKnapsackWithMaxValue(List<LootCandidate> items, int capacity, int maxAllowedTotalValue)
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

                    if (includeValue <= maxAllowedTotalValue && includeValue > dp[i, w])
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