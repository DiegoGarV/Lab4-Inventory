using System;
using UnityEngine;

[Serializable]
public class HouseDoorEntry
{
    public DoorController door;
    public string doorId;

    [NonSerialized] public DoorController.DoorLevel sceneStartDoorLevel;
}