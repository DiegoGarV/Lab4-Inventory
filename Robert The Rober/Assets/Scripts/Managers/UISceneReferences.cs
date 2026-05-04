using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISceneReferences : MonoBehaviour
{
    [Header("HUD")]
    public TMP_Text cashText;

    [Header("Sack UI")]
    public Animator sackAnimator;

    [Header("Controllers")]
    public MonoBehaviour firstPersonController;
    public MonoBehaviour actionManager;

    [Header("Runaway Screen")]
    public GameObject runawayScreen;
    public TMP_Text runawayCashText;
    public TMP_Text runawayItemsMoneyText;
    public TMP_Text runawayTotalText;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Main Menu")]
    public Button loadGameButton;
}