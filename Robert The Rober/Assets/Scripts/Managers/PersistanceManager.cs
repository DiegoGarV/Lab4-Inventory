using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance;

    [SerializeField] private Transform playerTransform;

    private string saveFilePath;
    private SaveData loadedData;
    private List<string> collectedPickupIds = new();

    public SaveData LoadedData => loadedData;
    public List<string> CollectedPickupIds => collectedPickupIds;

    public bool ShouldLoadGame { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    private void OnEnable()
    {
        Pickup.OnPickupCollected += HandlePickupCollected;
    }

    private void OnDisable()
    {
        Pickup.OnPickupCollected -= HandlePickupCollected;
    }

    private void HandlePickupCollected(Pickup pickup)
    {
        if (pickup == null) return;

        if (!string.IsNullOrEmpty(pickup.PickupId) && !collectedPickupIds.Contains(pickup.PickupId))
        {
            collectedPickupIds.Add(pickup.PickupId);
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public void SaveGame()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform == null)
        {
            Debug.LogError("No se encontró el jugador para guardar.");
            return;
        }

        if (MoneyAndObjectsController.Instance == null)
        {
            Debug.LogError("No existe MoneyAndObjectsController al guardar.");
            return;
        }

        SaveData data = new SaveData();

        data.cashScore = MoneyAndObjectsController.Instance.CashScore;
        data.storedLootValue = MoneyAndObjectsController.Instance.StoredLootValue;
        data.currentSackLoad = MoneyAndObjectsController.Instance.CurrentSackLoad;

        data.playerPosX = playerTransform.position.x;
        data.playerPosY = playerTransform.position.y;
        data.playerPosZ = playerTransform.position.z;
        data.playerRotY = playerTransform.eulerAngles.y;

        data.collectedPickupIds = new List<string>(collectedPickupIds);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        loadedData = data;

        Debug.Log("Juego guardado en: " + saveFilePath);
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

        collectedPickupIds = new List<string>(loadedData.collectedPickupIds);

        Debug.Log("Juego cargado desde: " + saveFilePath);
        return loadedData;
    }

    public void ClearRuntimeData()
    {
        loadedData = null;
        collectedPickupIds.Clear();
    }

    public void DeleteSave()
    {
        if (HasSaveFile())
        {
            File.Delete(saveFilePath);
        }

        ClearRuntimeData();
    }

    private void RemoveCollectedPickups(List<string> collectedIds)
    {
        if (collectedIds == null) return;

        Pickup[] allPickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None);

        foreach (Pickup pickup in allPickups)
        {
            if (collectedIds.Contains(pickup.PickupId))
            {
                Destroy(pickup.gameObject);
            }
        }
    }

    private void RestorePlayerTransform(SaveData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        player.transform.position = new Vector3(
            data.playerPosX,
            data.playerPosY,
            data.playerPosZ
        );

        Vector3 euler = player.transform.eulerAngles;
        euler.y = data.playerRotY;
        player.transform.eulerAngles = euler;
    }

    public void ApplyLoadedGame()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("No hay datos cargados para aplicar.");
            return;
        }

        RestorePlayerTransform(loadedData);

        if (MoneyAndObjectsController.Instance != null)
        {
            MoneyAndObjectsController.Instance.LoadFromSaveData(loadedData);
        }

        RemoveCollectedPickups(loadedData.collectedPickupIds);

        InventoryUIManager inventoryUI = FindFirstObjectByType<InventoryUIManager>();
        if (inventoryUI != null)
        {
            inventoryUI.LoadInventoryFromSave(loadedData.collectedPickupIds);
        }
    }

    public void PrepareNewGame()
    {
        ShouldLoadGame = false;
        loadedData = null;
        collectedPickupIds.Clear();
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