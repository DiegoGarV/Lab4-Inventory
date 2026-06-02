using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance;

    private string saveFilePath;
    private SaveData loadedData;

    public SaveData LoadedData => loadedData;
    public bool ShouldLoadGame { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        SavePlayerProgress(data);
        SaveWorldState(data);
        SavePlayerTransform(data);
        SaveCurrentHeistRun(data);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        loadedData = data;

        Debug.Log("Juego guardado en: " + saveFilePath);
    }

    private void SavePlayerProgress(SaveData data)
    {
        if (PlayerProgressManager.Instance == null)
            return;

        data.currentCurrency = PlayerProgressManager.Instance.CurrentCurrency;
        data.currentLevelName = PlayerProgressManager.Instance.CurrentLevelName;
        data.purchasedItems = new List<PurchasedStoreItemData>(PlayerProgressManager.Instance.PurchasedItems);
    }

    private void SaveWorldState(SaveData data)
    {
        if (WorldStateManager.Instance == null)
            return;

        data.isPlayerInsideHouse = WorldStateManager.Instance.IsPlayerInsideHouse;
        data.stolenThingsIds = new List<string>(WorldStateManager.Instance.StolenThingsIds);
        data.openDoorIds = new List<string>(WorldStateManager.Instance.OpenDoorIds);
        data.savedDoorStates = new List<SavedDoorState>(WorldStateManager.Instance.SavedDoorStates);
        data.housesWithActiveCamerasIds = new List<string>(WorldStateManager.Instance.HousesWithActiveCamerasIds);
        data.housesWithActiveDogIds = new List<string>(WorldStateManager.Instance.HousesWithActiveDogIds);
    }

    private void SavePlayerTransform(SaveData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        Transform playerTransform = player.transform;

        data.playerPosX = playerTransform.position.x;
        data.playerPosY = playerTransform.position.y;
        data.playerPosZ = playerTransform.position.z;
        data.playerRotY = playerTransform.eulerAngles.y;
    }

    private void SaveCurrentHeistRun(SaveData data)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "HeistScene")
            return;

        if (MoneyAndObjectsController.Instance == null)
            return;

        data.cashScore = MoneyAndObjectsController.Instance.CashScore;
        data.storedLootValue = MoneyAndObjectsController.Instance.StoredLootValue;
        data.currentSackLoad = MoneyAndObjectsController.Instance.CurrentSackLoad;
    }

    public SaveData LoadGame()
    {
        if (!HasSaveFile())
        {
            Debug.LogWarning("No existe archivo de guardado.");
            return null;
        }

        string json = File.ReadAllText(saveFilePath);
        loadedData = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("Juego cargado desde: " + saveFilePath);
        return loadedData;
    }

    public void ApplyLoadedGame()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("No hay datos cargados para aplicar.");
            return;
        }

        ApplyPlayerProgress(loadedData);
        ApplyWorldState(loadedData);
        RestorePlayerTransform(loadedData);
        ApplyCurrentHeistRun(loadedData);
    }

    private void ApplyPlayerProgress(SaveData data)
    {
        if (PlayerProgressManager.Instance == null)
            return;

        PlayerProgressManager.Instance.LoadFromSaveData(data);
    }

    private void ApplyWorldState(SaveData data)
    {
        if (WorldStateManager.Instance == null)
            return;

        WorldStateManager.Instance.LoadFromSaveData(data);
    }

    private void RestorePlayerTransform(SaveData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        player.transform.position = new Vector3(
            data.playerPosX,
            data.playerPosY,
            data.playerPosZ
        );

        Vector3 euler = player.transform.eulerAngles;
        euler.y = data.playerRotY;
        player.transform.eulerAngles = euler;
    }

    private void ApplyCurrentHeistRun(SaveData data)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "HeistScene")
            return;

        if (MoneyAndObjectsController.Instance == null)
            return;

        MoneyAndObjectsController.Instance.LoadFromSaveData(data);
    }

    public void DeleteSave()
    {
        if (HasSaveFile())
        {
            File.Delete(saveFilePath);
        }

        loadedData = null;
        ShouldLoadGame = false;
    }

    public void PrepareNewGame()
    {
        ShouldLoadGame = false;
        loadedData = null;
    }

    public void PrepareLoadGame()
    {
        LoadGame();
        ShouldLoadGame = loadedData != null;
    }

    public void ClearLoadFlag()
    {
        ShouldLoadGame = false;
    }
}