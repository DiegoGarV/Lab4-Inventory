using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // PlayerProgressManager
    public int currentCurrency;
    public string currentLevelName;
    public List<PurchasedStoreItemData> purchasedItems = new();

    // WorldStateManager
    public bool isPlayerInsideHouse;
    public List<string> stolenThingsIds = new();
    public List<string> openDoorIds = new();
    public List<SavedDoorState> savedDoorStates = new();
    public List<string> housesWithActiveCamerasIds = new();
    public List<string> housesWithActiveDogIds = new();

    // Posición del jugador
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public float playerRotY;

    // Estado de run actual en HeistScene
    public int cashScore;
    public int storedLootValue;
    public float currentSackLoad;
}