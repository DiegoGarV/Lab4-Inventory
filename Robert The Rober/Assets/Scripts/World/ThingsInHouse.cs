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
        DisableCamerasAtStart();
        DisableDogAtStart();
        ApplySavedDoorStates();
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

            EntityID entityId = entry.camera.GetComponent<EntityID>();

            if (entityId == null)
                entityId = entry.camera.GetComponentInParent<EntityID>();

            if (entityId == null)
                entityId = entry.camera.GetComponentInChildren<EntityID>();

            entry.cameraId = entityId != null ? entityId.ID : "";
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

    private void DisableCamerasAtStart()
    {
        foreach (HouseCameraEntry entry in cameras)
        {
            if (entry == null || entry.camera == null)
                continue;

            EntityID entityId = entry.camera.GetComponent<EntityID>();

            if (entityId == null)
                entityId = entry.camera.GetComponentInParent<EntityID>();

            if (entityId != null)
            {
                entityId.gameObject.SetActive(false);
            }
            else
            {
                entry.camera.gameObject.SetActive(false);
            }
        }
    }

    private void DisableDogAtStart()
    {
        if (dog != null)
        {
            dog.gameObject.SetActive(false);
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

            Debug.Log($"[{HouseId}] Door {entry.doorId} | CurrentLevel={entry.door.CurrentDoorLevel} | WasOpenLastRun={wasOpen}");
        }

        Debug.Log($"[{HouseId}] totalDoors={totalDoors} | openDoorsInThisHouse={openDoorsInThisHouse}");

        if (totalDoors == 0)
            return;

        if (openDoorsInThisHouse >= Mathf.CeilToInt(totalDoors / 2f))
        {
            Debug.Log($"[{HouseId}] Subiendo seguridad de puertas");
            foreach (HouseDoorEntry entry in doors)
            {
                if (entry == null || entry.door == null || string.IsNullOrEmpty(entry.doorId))
                    continue;

                Debug.Log($"Antes de subir: {entry.doorId} -> {entry.door.CurrentDoorLevel}");

                entry.door.ForceClosed();
                entry.door.UpgradeSecurityLevel();

                Debug.Log($"Después de subir: {entry.doorId} -> {entry.door.CurrentDoorLevel}");

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

            Debug.Log($"Puerta {entry.doorId} | IsOpen = {entry.door.IsOpen} | Baseline = {entry.sceneStartDoorLevel}");

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
}