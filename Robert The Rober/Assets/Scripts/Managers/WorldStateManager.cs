using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    [SerializeField] private bool isPlayerInsideHouse = false;
    [SerializeField] private List<string> stolenThingsIds = new();
    [SerializeField] private List<string> openDoorIds = new();
    [SerializeField] private List<SavedDoorState> savedDoorStates = new();
    [SerializeField] private List<string> housesWithActiveCamerasIds = new();
    [SerializeField] private List<string> housesWithActiveDogIds = new();

    public bool IsPlayerInsideHouse => isPlayerInsideHouse;
    public List<string> StolenThingsIds => stolenThingsIds;
    public List<string> OpenDoorIds => openDoorIds;
    public List<SavedDoorState> SavedDoorStates => savedDoorStates;
    public List<string> HousesWithActiveCamerasIds => housesWithActiveCamerasIds;
    public List<string> HousesWithActiveDogIds => housesWithActiveDogIds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TogglePlayerInsideHouse()
    {
        isPlayerInsideHouse = !isPlayerInsideHouse;
    }

    public bool IsThingStolen(string thingId)
    {
        if (string.IsNullOrEmpty(thingId))
            return false;

        return stolenThingsIds.Contains(thingId);
    }

    public void RegisterStolenThing(string thingId)
    {
        if (string.IsNullOrEmpty(thingId))
            return;

        if (!stolenThingsIds.Contains(thingId))
        {
            stolenThingsIds.Add(thingId);
        }
    }

    public void ClearWorldState()
    {
        isPlayerInsideHouse = false;
        stolenThingsIds.Clear();
        openDoorIds.Clear();
        savedDoorStates.Clear();
        housesWithActiveCamerasIds.Clear();
        housesWithActiveDogIds.Clear();
    }

    public void SaveCurrentlyOpenDoor(string doorId)
    {
        if (string.IsNullOrEmpty(doorId))
            return;

        if (!openDoorIds.Contains(doorId))
        {
            openDoorIds.Add(doorId);
        }
    }

    public void ClearOpenDoorIds()
    {
        openDoorIds.Clear();
    }

    public DoorController.DoorLevel GetSavedDoorLevel(string doorId, DoorController.DoorLevel fallbackLevel)
    {
        if (string.IsNullOrEmpty(doorId))
            return fallbackLevel;

        SavedDoorState state = savedDoorStates.Find(x => x.doorId == doorId);
        return state != null ? state.savedDoorLevel : fallbackLevel;
    }

    public void SetSavedDoorLevel(string doorId, DoorController.DoorLevel newLevel)
    {
        if (string.IsNullOrEmpty(doorId))
            return;

        SavedDoorState existing = savedDoorStates.Find(x => x.doorId == doorId);

        if (existing != null)
        {
            existing.savedDoorLevel = newLevel;
        }
        else
        {
            savedDoorStates.Add(new SavedDoorState(doorId, newLevel));
        }
    }

    public bool HasActiveCamerasInHouse(string houseId)
    {
        if (string.IsNullOrEmpty(houseId))
            return false;

        return housesWithActiveCamerasIds.Contains(houseId);
    }

    public void ActivateCamerasInHouse(string houseId)
    {
        if (string.IsNullOrEmpty(houseId))
            return;

        if (!housesWithActiveCamerasIds.Contains(houseId))
        {
            housesWithActiveCamerasIds.Add(houseId);
        }
    }

    public bool HasActiveDogInHouse(string houseId)
    {
        if (string.IsNullOrEmpty(houseId))
            return false;

        return housesWithActiveDogIds.Contains(houseId);
    }

    public void ActivateDogInHouse(string houseId)
    {
        if (string.IsNullOrEmpty(houseId))
            return;

        if (!housesWithActiveDogIds.Contains(houseId))
        {
            housesWithActiveDogIds.Add(houseId);
        }
    }

    public void LoadFromSaveData(SaveData data)
    {
        if (data == null) return;

        isPlayerInsideHouse = data.isPlayerInsideHouse;
        stolenThingsIds = new List<string>(data.stolenThingsIds);
        openDoorIds = new List<string>(data.openDoorIds);
        savedDoorStates = new List<SavedDoorState>(data.savedDoorStates);
        housesWithActiveCamerasIds = new List<string>(data.housesWithActiveCamerasIds);
        housesWithActiveDogIds = new List<string>(data.housesWithActiveDogIds);
    }
}