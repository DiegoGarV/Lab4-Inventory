using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private TMP_Text cashText;
    private Animator sackAnimator;
    private MonoBehaviour firstPersonController;
    private MonoBehaviour actionController;
    private GameObject runawayScreen;
    private TMP_Text runawayCashText;
    private TMP_Text runawayItemsMoneyText;
    private TMP_Text runawayTotalText;
    private GameObject pauseMenu;
    private Button loadGameButton;
    private MoneyAndObjectsController boundMoneyController;
    private GameObject returnToTownPrompt;
    private TMP_Text currencyText;
    private GameObject buyItemPrompt;
    private TMP_Text buyItemNameText;
    private UnityEngine.UI.Image buyItemIconImage;
    private TMP_Text buyItemDescriptionText;
    private TMP_Text buyItemPriceText;
    private TMP_Text currencyInText;
    private Button buyItemButton;
    public GameObject purchasedItemsPanel;
    public MonoBehaviour purchasedItemsUIController;
    public GameObject stolenItemsPanel;
    public MonoBehaviour stolenItemsUIController;

    private StoreItemBase currentStoreItem;
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (boundMoneyController != null)
        {
            boundMoneyController.OnCashChanged -= UpdateCashText;
            boundMoneyController.OnSackChanged -= UpdateSackBar;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            TogglePurchasedItemsPanel();
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleStolenItemsPanel();
        }
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name != "BootstrapScene")
        {
            BindSceneReferences(currentScene);
            RebindMoneyController();
            ApplySceneUIState(currentScene);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "BootstrapScene")
            return;
        
        BindSceneReferences(scene);
        RebindMoneyController();
        ApplySceneUIState(scene);

        if (PersistenceManager.Instance != null && PersistenceManager.Instance.ShouldLoadGame)
        {
            PersistenceManager.Instance.ApplyLoadedGame();
            PersistenceManager.Instance.ClearLoadFlag();
        }
    }

    private void BindSceneReferences(Scene scene)
    {
        ClearSceneReferences();

        GameObject[] roots = scene.GetRootGameObjects();
        UISceneReferences refs = null;

        foreach (GameObject root in roots)
        {
            refs = root.GetComponentInChildren<UISceneReferences>(true);
            if (refs != null)
                break;
        }

        if (refs == null)
        {
            Debug.LogWarning($"UIManager: no se encontró UISceneReferences en la escena {scene.name}");
            return;
        }

        cashText = refs.cashText;
        sackAnimator = refs.sackAnimator;
        firstPersonController = refs.firstPersonController;
        actionController = refs.actionController;
        runawayScreen = refs.runawayScreen;
        runawayCashText = refs.runawayCashText;
        runawayItemsMoneyText = refs.runawayItemsMoneyText;
        runawayTotalText = refs.runawayTotalText;
        pauseMenu = refs.pauseMenu;
        loadGameButton = refs.loadGameButton;
        returnToTownPrompt = refs.returnToTownPrompt;
        currencyText = refs.currencyText;
        buyItemPrompt = refs.buyItemPrompt;
        buyItemNameText = refs.buyItemNameText;
        buyItemIconImage = refs.buyItemIconImage;
        buyItemDescriptionText = refs.buyItemDescriptionText;
        buyItemPriceText = refs.buyItemPriceText;
        currencyInText = refs.currencyInText;
        buyItemButton = refs.buyItemButton;
        purchasedItemsPanel = refs.purchasedItemsPanel;
        purchasedItemsUIController = refs.purchasedItemsUIController;
        stolenItemsPanel = refs.stolenItemsPanel;
        stolenItemsUIController = refs.stolenItemsUIController;

        Debug.Log($"UIManager: referencias enlazadas para escena {scene.name}");
    }

    private void ClearSceneReferences()
    {
        cashText = null;
        sackAnimator = null;
        firstPersonController = null;
        actionController = null;
        runawayScreen = null;
        runawayCashText = null;
        runawayItemsMoneyText = null;
        runawayTotalText = null;
        pauseMenu = null;
        loadGameButton = null;
        returnToTownPrompt = null;
        currencyText = null;
        buyItemPrompt = null;
        buyItemNameText = null;
        buyItemIconImage = null;
        buyItemDescriptionText = null;
        buyItemPriceText = null;
        currencyInText = null;
        currentStoreItem = null;
        buyItemButton = null;
        purchasedItemsPanel = null;
        purchasedItemsUIController = null;
        stolenItemsPanel = null;
        stolenItemsUIController = null;
    }

    private void ApplySceneUIState(Scene scene)
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (scene.name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (loadGameButton != null)
            {
                bool hasSave = PersistenceManager.Instance != null && PersistenceManager.Instance.HasSaveFile();
                loadGameButton.interactable = hasSave;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (runawayScreen != null)
            runawayScreen.SetActive(false);

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (returnToTownPrompt != null)
            returnToTownPrompt.SetActive(false);

        if (MoneyAndObjectsController.Instance != null)
        {
            UpdateCashText(MoneyAndObjectsController.Instance.CashScore);
            UpdateSackBar(
                MoneyAndObjectsController.Instance.CurrentSackLoad,
                MoneyAndObjectsController.Instance.MaxSackLoad
            );
        }

        if (buyItemPrompt != null)
            buyItemPrompt.SetActive(false);

        if (purchasedItemsPanel != null)
            purchasedItemsPanel.SetActive(false);

        if (stolenItemsPanel != null)
            stolenItemsPanel.SetActive(false);

        UpdateStoreCurrency();
    }

    private void RebindMoneyController()
    {
        if (boundMoneyController != null)
        {
            boundMoneyController.OnCashChanged -= UpdateCashText;
            boundMoneyController.OnSackChanged -= UpdateSackBar;
        }

        boundMoneyController = MoneyAndObjectsController.Instance;

        if (boundMoneyController != null)
        {
            boundMoneyController.OnCashChanged += UpdateCashText;
            boundMoneyController.OnSackChanged += UpdateSackBar;

            UpdateCashText(boundMoneyController.CashScore);
            UpdateSackBar(boundMoneyController.CurrentSackLoad, boundMoneyController.MaxSackLoad);

            Debug.Log("UIManager: rebind a MoneyAndObjectsController exitoso.");
        }
        else
        {
            Debug.LogWarning("UIManager: no encontró MoneyAndObjectsController para enlazar.");
        }
    }

    private void UpdateCashText(int currentCash)
    {
        if (cashText != null)
            cashText.text = "Cash: $" + currentCash;
    }

    private void UpdateSackBar(float currentLoad, float maxLoad)
    {
        if (sackAnimator == null) return;

        float normalizedLoad = Mathf.InverseLerp(0f, maxLoad, currentLoad);
        sackAnimator.SetFloat("Storage", normalizedLoad);
    }

    public void ShowRunawayScreen()
    {
        UpdateRunawayScreenValues();

        if (runawayScreen != null)
            runawayScreen.SetActive(true);

        if (firstPersonController != null)
            firstPersonController.enabled = false;

        if (actionController != null)
            actionController.enabled = false;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateRunawayScreenValues()
    {
        if (MoneyAndObjectsController.Instance == null) return;

        int cashCollected = MoneyAndObjectsController.Instance.CashScore;
        int itemsMoneyCollected = MoneyAndObjectsController.Instance.StoredLootValue;
        int totalCollected = cashCollected + itemsMoneyCollected;

        if (runawayCashText != null)
            runawayCashText.text = $"${cashCollected}";

        if (runawayItemsMoneyText != null)
            runawayItemsMoneyText.text = $"${itemsMoneyCollected}";

        if (runawayTotalText != null)
            runawayTotalText.text = $"${totalCollected}";
    }

    public void TogglePauseMenu()
    {
        if (runawayScreen != null && runawayScreen.activeSelf)
            return;

        isPaused = !isPaused;

        if (pauseMenu != null)
            pauseMenu.SetActive(isPaused);

        if (firstPersonController != null)
            firstPersonController.enabled = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (firstPersonController != null)
            firstPersonController.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SafeCloseGame()
    {
        Time.timeScale = 1f;

        if (PersistenceManager.Instance != null)
            PersistenceManager.Instance.SaveGame();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NewGame()
    {
        Time.timeScale = 1f;

        if (PersistenceManager.Instance != null)
            PersistenceManager.Instance.PrepareNewGame();

        if (PlayerProgressManager.Instance != null)
            PlayerProgressManager.Instance.SetCurrentLevel("MorningLevel");

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithLoadingScreen("MorningLevel");
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;

        if (PersistenceManager.Instance == null || !PersistenceManager.Instance.HasSaveFile())
            return;

        PersistenceManager.Instance.PrepareLoadGame();
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithLoadingScreen("MorningLevel");
    }

    public void GoToStore()
    {
        Time.timeScale = 1f;

        if (MoneyAndObjectsController.Instance != null && PlayerProgressManager.Instance != null)
        {
            int totalCollected =
                MoneyAndObjectsController.Instance.CashScore +
                MoneyAndObjectsController.Instance.StoredLootValue;

            PlayerProgressManager.Instance.AddCurrency(totalCollected);
            PlayerProgressManager.Instance.SetCurrentLevel("Shop");
        }

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithLoadingScreen("Shop");
    }

    public void OpenSettings()
    {
    }

    public void ShowReturnToTownPrompt()
    {
        if (returnToTownPrompt != null)
            returnToTownPrompt.SetActive(true);

        if (firstPersonController != null)
            firstPersonController.enabled = false;

        if (actionController != null)
            actionController.enabled = false;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ConfirmReturnToTown()
    {
        Time.timeScale = 1f;

        if (PlayerProgressManager.Instance != null)
            PlayerProgressManager.Instance.SetCurrentLevel("AfternoonLevel");

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadSceneWithLoadingScreen("AfternoonLevel");
    }

    public void CancelReturnToTown()
    {
        if (returnToTownPrompt != null)
            returnToTownPrompt.SetActive(false);

        if (firstPersonController != null)
            firstPersonController.enabled = true;

        if (actionController != null)
            actionController.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateStoreCurrency()
    {
        if (currencyText == null) return;
        if (PlayerProgressManager.Instance == null) return;

        currencyText.text = "Currency: $" + PlayerProgressManager.Instance.CurrentCurrency;
    }

    public void ShowBuyItemPrompt(StoreItemBase storeItem)
    {
        if (storeItem == null) return;

        currentStoreItem = storeItem;

        if (buyItemNameText != null)
            buyItemNameText.text = storeItem.ItemName;

        if (buyItemIconImage != null)
        {
            buyItemIconImage.sprite = storeItem.ItemIcon;
            buyItemIconImage.enabled = storeItem.ItemIcon != null;
            buyItemIconImage.preserveAspect = true;
        }

        if (buyItemDescriptionText != null)
            buyItemDescriptionText.text = storeItem.EffectDescription;

        if (buyItemPriceText != null)
            buyItemPriceText.text = "Price: $" + storeItem.ItemPrice;

        if (currencyInText != null)
            currencyInText.text = "Currency: $" + PlayerProgressManager.Instance.CurrentCurrency;

        RefreshBuyItemUI();
        
        if (buyItemPrompt != null)
            buyItemPrompt.SetActive(true);

        if (firstPersonController != null)
            firstPersonController.enabled = false;

        if (actionController != null)
            actionController.enabled = false;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseBuyItemPrompt()
    {
        currentStoreItem = null;

        if (buyItemPrompt != null)
            buyItemPrompt.SetActive(false);

        if (firstPersonController != null)
            firstPersonController.enabled = true;

        if (actionController != null)
            actionController.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void RefreshBuyItemUI()
    {
        if (currentStoreItem == null) return;

        int currentCurrency = PlayerProgressManager.Instance != null
            ? PlayerProgressManager.Instance.CurrentCurrency
            : 0;

        if (buyItemPriceText != null)
            buyItemPriceText.text = "Price: $" + currentStoreItem.ItemPrice;

        if (currencyInText != null)
            currencyInText.text = "Currency: $" + currentCurrency;

        if (buyItemButton != null)
            buyItemButton.interactable = currentStoreItem.CanBePurchased(currentCurrency);
    }

    public void BuyCurrentStoreItem()
    {
        if (currentStoreItem == null) return;
        if (PlayerProgressManager.Instance == null) return;

        int itemPrice = currentStoreItem.ItemPrice;

        if (!currentStoreItem.CanBePurchased(PlayerProgressManager.Instance.CurrentCurrency))
        {
            RefreshBuyItemUI();
            return;
        }

        bool spentSuccessfully = PlayerProgressManager.Instance.SpendCurrency(itemPrice);
        if (!spentSuccessfully)
        {
            RefreshBuyItemUI();
            return;
        }

        PlayerProgressManager.Instance.RegisterPurchase(currentStoreItem);

        bool shouldDisappear = currentStoreItem.DestroyOnPurchase;

        currentStoreItem.Purchase();

        UpdateStoreCurrency();

        if (shouldDisappear)
        {
            CloseBuyItemPrompt();
        }
        else
        {
            RefreshBuyItemUI();
        }
    }

    public void TogglePurchasedItemsPanel()
    {
        if (purchasedItemsPanel == null)
        {
            Debug.LogWarning("UIManager: purchasedItemsPanel es null.");
            return;
        }

        if (stolenItemsPanel != null && stolenItemsPanel.activeSelf)
            return;

        bool willOpen = !purchasedItemsPanel.activeSelf;
        Debug.Log("UIManager: TogglePurchasedItemsPanel -> " + willOpen);

        purchasedItemsPanel.SetActive(willOpen);

        if (willOpen)
        {
            if (firstPersonController != null)
                firstPersonController.enabled = false;

            if (actionController != null)
                actionController.enabled = false;

            if (purchasedItemsUIController != null)
            {
                PurchasedItemsUIController controller = purchasedItemsUIController as PurchasedItemsUIController;
                if (controller != null)
                {
                    controller.RefreshPurchasedItemsUI();
                }
            }

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (firstPersonController != null)
                firstPersonController.enabled = true;

            if (actionController != null)
                actionController.enabled = true;

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ToggleStolenItemsPanel()
    {
        if (stolenItemsPanel == null)
        {
            Debug.LogWarning("UIManager: stolenItemsPanel es null.");
            return;
        }

        if (purchasedItemsPanel != null && purchasedItemsPanel.activeSelf)
            return;

        bool willOpen = !stolenItemsPanel.activeSelf;
        Debug.Log("UIManager: ToggleStolenItemsPanel -> " + willOpen);

        stolenItemsPanel.SetActive(willOpen);

        if (willOpen)
        {
            if (firstPersonController != null)
                firstPersonController.enabled = false;

            if (actionController != null)
                actionController.enabled = false;

            if (stolenItemsUIController != null)
            {
                InventoryUIController controller = stolenItemsUIController as InventoryUIController;
                if (controller != null)
                {
                    controller.RefreshStolenItemsUI();
                }
            }

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (firstPersonController != null)
                firstPersonController.enabled = true;

            if (actionController != null)
                actionController.enabled = true;

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}