using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    [SerializeField] private bool isPlayerInsideHouse = false;
    [SerializeField] private List<string> stolenThingsIds = new();
    [SerializeField] private List<string> openDoorIds = new();
    [SerializeField] private List<SavedDoorState> savedDoorStates = new();

    public bool IsPlayerInsideHouse => isPlayerInsideHouse;
    public List<string> StolenThingsIds => stolenThingsIds;
    public List<string> OpenDoorIds => openDoorIds;
    public List<SavedDoorState> SavedDoorStates => savedDoorStates;

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
}