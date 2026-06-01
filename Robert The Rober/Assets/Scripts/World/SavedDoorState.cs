using System;
using UnityEngine;

[Serializable]
public class SavedDoorState
{
    public string doorId;
    public DoorController.DoorLevel savedDoorLevel;

    public SavedDoorState(string doorId, DoorController.DoorLevel savedDoorLevel)
    {
        this.doorId = doorId;
        this.savedDoorLevel = savedDoorLevel;
    }
}