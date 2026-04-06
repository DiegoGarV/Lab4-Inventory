using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TMP_Text cashText;

    [Header("Sack UI")]
    [SerializeField] private Animator sackAnimator;
    [SerializeField] private string sackParameterName = "SackNormalized";

    [Header("Controllers")]
    [SerializeField] private MonoBehaviour firstPersonController;
    [SerializeField] private MonoBehaviour actionManager;

    [Header("Runaway Screen")]
    [SerializeField] private GameObject runawayScreen;

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
        if (MoneyAndObjectsController.Instance != null)
        {
            MoneyAndObjectsController.Instance.OnCashChanged += UpdateCashText;
            MoneyAndObjectsController.Instance.OnSackChanged += UpdateSackBar;
        }
    }

    private void OnDisable()
    {
        if (MoneyAndObjectsController.Instance != null)
        {
            MoneyAndObjectsController.Instance.OnCashChanged -= UpdateCashText;
            MoneyAndObjectsController.Instance.OnSackChanged -= UpdateSackBar;
        }
    }

    private void Start()
    {
        if (runawayScreen != null)
        {
            runawayScreen.SetActive(false);
        }

        if (MoneyAndObjectsController.Instance != null)
        {
            UpdateCashText(MoneyAndObjectsController.Instance.CashScore);
            UpdateSackBar(
                MoneyAndObjectsController.Instance.CurrentSackLoad,
                MoneyAndObjectsController.Instance.MaxSackLoad
            );
        }
    }

    private void UpdateCashText(int currentCash)
    {
        if (cashText != null)
        {
            cashText.text = "Cash: $" + currentCash;
        }
    }

    private void UpdateSackBar(float currentLoad, float maxLoad)
    {
        if (sackAnimator == null) return;

        float normalizedLoad = Mathf.InverseLerp(0f, maxLoad, currentLoad);
        sackAnimator.SetFloat(sackParameterName, normalizedLoad);
    }

    public void ShowRunawayScreen()
    {
        if (runawayScreen != null)
        {
            runawayScreen.SetActive(true);
        }

        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        if (actionManager != null)
        {
            actionManager.enabled = false;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseGame()
    {
        Time.timeScale = 1f;

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
}