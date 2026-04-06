using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // Money and Objects Score
    public int cashScore;
    public int storedLootValue;
    public float currentSackLoad;

    // Player Position and Rotation
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public float playerRotY;

    // Collected items for inventory and world state
    public List<string> collectedPickupIds = new();
}