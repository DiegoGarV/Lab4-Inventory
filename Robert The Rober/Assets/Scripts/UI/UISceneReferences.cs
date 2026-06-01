using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISceneReferences : MonoBehaviour
{
    [Header("Heist HUD")]
    public TMP_Text cashText;
    public Animator sackAnimator;
    public TMP_Text sackLoadText;
    public TMP_Text sackableHintText;
    public TMP_Text sackableValueText;

    [Header("Controllers")]
    public MonoBehaviour firstPersonController;
    public MonoBehaviour actionController;

    [Header("Runaway Screen")]
    public GameObject runawayScreen;
    public TMP_Text runawayCashText;
    public TMP_Text runawayItemsMoneyText;
    public TMP_Text runawayTotalText;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    
    [Header("Caught Screen")]
    public GameObject caughtScreen;

    [Header("Main Menu")]
    public Button loadGameButton;

    [Header("Store Exit")]
    public GameObject returnToTownPrompt;

    [Header("Store HUD")]
    public TMP_Text currencyText;

    [Header("Buy Item")]
    public GameObject buyItemPrompt;
    public TMP_Text buyItemNameText;
    public Image buyItemIconImage;
    public TMP_Text buyItemDescriptionText;
    public TMP_Text buyItemPriceText;
    public TMP_Text currencyInText;
    public UnityEngine.UI.Button buyItemButton;

    [Header("Inventories")]
    public GameObject purchasedItemsPanel;
    public MonoBehaviour purchasedItemsUIController;
    public GameObject stolenItemsPanel;
    public MonoBehaviour stolenItemsUIController;

    [Header("Buttons")]
    public Button pauseFirstButton;
    public Button buyPromptFirstButton;
    public Button toNextSceneFirstButton;

    
}