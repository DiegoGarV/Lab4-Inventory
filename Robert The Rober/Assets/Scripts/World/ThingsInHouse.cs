using System.Collections.Generic;
using UnityEngine;

public class ThingsInHouse : MonoBehaviour
{
    [Header("House Info")]
    [SerializeField] private string houseId;

    [Header("Loot")]
    [SerializeField] private List<HouseLootEntry> lootItems = new();

    [Header("Doors")]
    [SerializeField] private List<HouseDoorEntry> doors = new();

    [Header("Security")]
    [SerializeField] private List<HouseCameraEntry> cameras = new();
    [SerializeField] private PowerBoxController powerBox;
    [SerializeField] private string powerBoxId;
    [SerializeField] private DogController dog;
    [SerializeField] private string dogId;

    [Header("Camera Security Rule")]
    [SerializeField] private int cameraActivationBagCapacity = 150;
    [SerializeField] private int cameraValueIgnoreAbove = 30000;
    [SerializeField] private float cameraActivationPercent = 0.60f;

    [Header("Dog Security Rule")]
    [SerializeField] private float dogActivationWeightThreshold = 50f;

    public string HouseId => houseId;
    public List<HouseLootEntry> LootItems => lootItems;
    public List<HouseDoorEntry> Doors => doors;
    public List<HouseCameraEntry> Cameras => cameras;
    public PowerBoxController PowerBox => powerBox;
    public string PowerBoxId => powerBoxId;
    public DogController Dog => dog;
    public string DogId => dogId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        RefreshIds();
    }
#endif

    private void Start()
    {
        ApplyStolenLootState();
        ApplyCameraState();
        ApplyDogState();
        ApplySavedDoorStates();
        EvaluateCameraSecurityUpgrade();
        EvaluateDogSecurityUpgrade();
    }

    private void RefreshIds()
    {
        RefreshLootIds();
        RefreshDoorIds();
        RefreshCameraIds();
        RefreshPowerBoxId();
        RefreshDogId();
    }

    private void RefreshLootIds()
    {
        foreach (HouseLootEntry entry in lootItems)
        {
            if (entry == null || entry.pickup == null)
                continue;

            EntityID entityId = entry.pickup.GetComponent<EntityID>();

            if (entityId == null)
                entityId = entry.pickup.GetComponentInParent<EntityID>();

            entry.pickupId = entityId != null ? entityId.ID : "";
        }
    }

    private void RefreshDoorIds()
    {
        foreach (HouseDoorEntry entry in doors)
        {
            if (entry == null || entry.door == null)
                continue;

            EntityID entityId = entry.door.GetComponent<EntityID>();

            if (entityId == null)
                entityId = entry.door.GetComponentInParent<EntityID>();

            if (entityId == null)
                entityId = entry.door.GetComponentInChildren<EntityID>();

            entry.doorId = entityId != null ? entityId.ID : "";
        }
    }

    private void RefreshCameraIds()
    {
        foreach (HouseCameraEntry entry in cameras)
        {
            if (entry == null || entry.camera == null)
                continue;

            if (entry.cameraRoot == null)
            {
                EntityID entityId = entry.camera.GetComponent<EntityID>();

                if (entityId == null)
                    entityId = entry.camera.GetComponentInParent<EntityID>();

                if (entityId == null)
                    entityId = entry.camera.GetComponentInChildren<EntityID>();

                if (entityId != null)
                    entry.cameraRoot = entityId.gameObject;
            }

            if (entry.cameraRoot != null)
            {
                EntityID entityId = entry.cameraRoot.GetComponent<EntityID>();
                entry.cameraId = entityId != null ? entityId.ID : "";
            }
            else
            {
                entry.cameraId = "";
            }
        }
    }

    private void RefreshPowerBoxId()
    {
        if (powerBox == null)
        {
            powerBoxId = "";
            return;
        }

        EntityID entityId = powerBox.GetComponent<EntityID>();

        if (entityId == null)
            entityId = powerBox.GetComponentInParent<EntityID>();

        if (entityId == null)
            entityId = powerBox.GetComponentInChildren<EntityID>();

        powerBoxId = entityId != null ? entityId.ID : "";
    }

    private void RefreshDogId()
    {
        if (dog == null)
        {
            dogId = "";
            return;
        }

        EntityID entityId = dog.GetComponent<EntityID>();

        if (entityId == null)
            entityId = dog.GetComponentInParent<EntityID>();

        if (entityId == null)
            entityId = dog.GetComponentInChildren<EntityID>();

        dogId = entityId != null ? entityId.ID : "";
    }

    public void ApplyStolenLootState()
    {
        if (WorldStateManager.Instance == null)
            return;

        foreach (HouseLootEntry entry in lootItems)
        {
            if (entry == null || entry.pickup == null)
                continue;

            if (WorldStateManager.Instance.IsThingStolen(entry.pickupId))
            {
                Destroy(entry.pickup.gameObject);
            }
        }
    }

    private void ApplySavedDoorStates()
    {
        if (WorldStateManager.Instance == null)
            return;

        foreach (HouseDoorEntry entry in doors)
        {
            if (entry == null || entry.door == null || string.IsNullOrEmpty(entry.doorId))
                continue;

            DoorController.DoorLevel savedLevel = WorldStateManager.Instance.GetSavedDoorLevel(
                entry.doorId,
                entry.door.CurrentDoorLevel
            );

            entry.door.SetDoorLevel(savedLevel);
            entry.sceneStartDoorLevel = savedLevel;
        }
    }

    public void EvaluateOpenDoorsSecurityUpgrade()
    {
        if (WorldStateManager.Instance == null)
            return;

        int totalDoors = 0;
        int openDoorsInThisHouse = 0;

        foreach (HouseDoorEntry entry in doors)
        {
            if (entry == null || entry.door == null || string.IsNullOrEmpty(entry.doorId))
                continue;

            totalDoors++;

            bool wasOpen = WorldStateManager.Instance.OpenDoorIds.Contains(entry.doorId);

            if (wasOpen)
            {
                openDoorsInThisHouse++;
            }
        }

        if (totalDoors == 0)
            return;

        if (openDoorsInThisHouse >= Mathf.CeilToInt(totalDoors / 2f))
        {
            foreach (HouseDoorEntry entry in doors)
            {
                if (entry == null || entry.door == null || string.IsNullOrEmpty(entry.doorId))
                    continue;

                entry.door.ForceClosed();
                entry.door.UpgradeSecurityLevel();

                WorldStateManager.Instance.SetSavedDoorLevel(
                    entry.doorId,
                    entry.door.CurrentDoorLevel
                );

                entry.sceneStartDoorLevel = entry.door.CurrentDoorLevel;
            }
        }
    }

    public void ReportOpenDoorsToWorldState()
    {
        if (WorldStateManager.Instance == null)
            return;

        foreach (HouseDoorEntry entry in doors)
        {
            if (entry == null || entry.door == null || string.IsNullOrEmpty(entry.doorId))
                continue;

            if (entry.door.IsOpen)
            {
                WorldStateManager.Instance.SaveCurrentlyOpenDoor(entry.doorId);
            }

            // Guardar el nivel persistente/base de esta corrida, no el nivel modificado por el jugador
            WorldStateManager.Instance.SetSavedDoorLevel(
                entry.doorId,
                entry.sceneStartDoorLevel
            );
        }
    }

    private void ApplyCameraState()
    {
        bool shouldBeActive =
            WorldStateManager.Instance != null &&
            WorldStateManager.Instance.HasActiveCamerasInHouse(houseId);

        foreach (HouseCameraEntry entry in cameras)
        {
            if (entry == null || entry.camera == null)
                continue;

            entry.cameraRoot.SetActive(shouldBeActive);
            Debug.Log($"[{houseId}] Camera root {entry.cameraRoot.name} -> active = {shouldBeActive}");
        }
    }

    private int CalculateStolenValueInThisHouse()
    {
        if (WorldStateManager.Instance == null)
            return 0;

        int total = 0;

        foreach (HouseLootEntry entry in lootItems)
        {
            if (entry == null || entry.pickup == null || string.IsNullOrEmpty(entry.pickupId))
                continue;

            if (!WorldStateManager.Instance.IsThingStolen(entry.pickupId))
                continue;

            if (entry.pickup.MonetaryValue > cameraValueIgnoreAbove)
                continue;

            total += entry.pickup.MonetaryValue;
        }

        return total;
    }

    private int CalculateBestPossibleLootValueForCameras()
    {
        List<Pickup> validLoot = new();

        foreach (HouseLootEntry entry in lootItems)
        {
            if (entry == null || entry.pickup == null)
                continue;

            if (entry.pickup.MonetaryValue > cameraValueIgnoreAbove)
                continue;

            if (entry.pickup.SackValue <= 0)
                continue;

            validLoot.Add(entry.pickup);
        }

        int capacity = cameraActivationBagCapacity;
        int[,] dp = new int[validLoot.Count + 1, capacity + 1];

        for (int i = 1; i <= validLoot.Count; i++)
        {
            int weight = Mathf.RoundToInt(validLoot[i - 1].SackValue);
            int value = validLoot[i - 1].MonetaryValue;

            for (int w = 0; w <= capacity; w++)
            {
                dp[i, w] = dp[i - 1, w];

                if (weight <= w)
                {
                    dp[i, w] = Mathf.Max(
                        dp[i, w],
                        dp[i - 1, w - weight] + value
                    );
                }
            }
        }

        return dp[validLoot.Count, capacity];
    }

    private void EvaluateCameraSecurityUpgrade()
    {
        if (WorldStateManager.Instance == null)
            return;

        if (WorldStateManager.Instance.HasActiveCamerasInHouse(houseId))
            return;

        int stolenValue = CalculateStolenValueInThisHouse();
        int bestPossibleValue = CalculateBestPossibleLootValueForCameras();

        if (bestPossibleValue <= 0)
            return;

        float stolenPercent = (float)stolenValue / bestPossibleValue;

        Debug.Log(
            $"[{houseId}] Camera check | stolenValue={stolenValue} | bestPossibleValue={bestPossibleValue} | percent={stolenPercent:P0}"
        );

        if (stolenPercent > cameraActivationPercent)
        {
            WorldStateManager.Instance.ActivateCamerasInHouse(houseId);
            ApplyCameraState();

            Debug.Log($"[{houseId}] Cámaras activadas.");
        }
    }

    private GameObject GetCameraRootObject(SecurityCameraController cameraController)
    {
        if (cameraController == null)
            return null;

        EntityID entityId = cameraController.GetComponent<EntityID>();

        if (entityId == null)
            entityId = cameraController.GetComponentInParent<EntityID>();

        if (entityId == null)
            entityId = cameraController.GetComponentInChildren<EntityID>();

        if (entityId != null)
            return entityId.gameObject;

        return cameraController.gameObject;
    }

    private void ApplyDogState()
    {
        if (dog == null)
            return;

        bool shouldBeActive =
            WorldStateManager.Instance != null &&
            WorldStateManager.Instance.HasActiveDogInHouse(houseId);

        dog.gameObject.SetActive(shouldBeActive);
    }

    private void EvaluateDogSecurityUpgrade()
    {
        if (WorldStateManager.Instance == null)
            return;

        if (WorldStateManager.Instance.HasActiveDogInHouse(houseId))
            return;

        foreach (HouseLootEntry entry in lootItems)
        {
            if (entry == null || entry.pickup == null || string.IsNullOrEmpty(entry.pickupId))
                continue;

            if (!WorldStateManager.Instance.IsThingStolen(entry.pickupId))
                continue;

            if (entry.pickup.SackValue >= dogActivationWeightThreshold)
            {
                WorldStateManager.Instance.ActivateDogInHouse(houseId);
                ApplyDogState();

                Debug.Log($"[{houseId}] Perro activado por objeto pesado: {entry.pickup.name} | Peso: {entry.pickup.SackValue}");
                return;
            }
        }
    }
}