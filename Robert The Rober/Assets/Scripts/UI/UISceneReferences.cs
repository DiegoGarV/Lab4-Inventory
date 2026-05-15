using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISceneReferences : MonoBehaviour
{
    [Header("Heist HUD")]
    public TMP_Text cashText;
    public Animator sackAnimator;

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
}