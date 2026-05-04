using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform barContainer;
    [SerializeField] private RectTransform barFill;
    [SerializeField] private RectTransform thiefIcon;
    [SerializeField] private TMP_Text loadingText;

    [Header("Animation")]
    [SerializeField] private float fillLerpSpeed = 0.75f;
    [SerializeField] private float minimumLoadingDuration = 1.5f;
    [SerializeField] private float thiefOffset = 12f;

    private float realProgress = 0f;
    private float displayedProgress = 0f;
    private float elapsedTime = 0f;
    private bool allowFinish = false;

    public bool IsVisuallyComplete => displayedProgress >= 0.999f;

    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;

        AnimateLoadingText();

        float visualTarget = realProgress;

        // Evita que termine demasiado abrupto antes del tiempo mínimo
        if (!allowFinish)
        {
            visualTarget = Mathf.Min(realProgress, 0.9f);

            if (elapsedTime >= minimumLoadingDuration && realProgress >= 0.9f)
            {
                allowFinish = true;
            }
        }

        displayedProgress = Mathf.MoveTowards(
            displayedProgress,
            visualTarget,
            fillLerpSpeed * Time.unscaledDeltaTime
        );

        UpdateBarVisuals();

        if (allowFinish && realProgress >= 1f)
        {
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                1f,
                (fillLerpSpeed * 1.5f) * Time.unscaledDeltaTime
            );

            UpdateBarVisuals();
        }
    }

    public void SetRealProgress(float normalizedProgress)
    {
        realProgress = Mathf.Clamp01(normalizedProgress);
    }

    public void MarkLoadComplete()
    {
        realProgress = 1f;
        allowFinish = true;
    }

    public void ResetScreen()
    {
        realProgress = 0f;
        displayedProgress = 0f;
        elapsedTime = 0f;
        allowFinish = false;
        UpdateBarVisuals();
    }

    private void UpdateBarVisuals()
    {
        if (barContainer == null || barFill == null) return;

        float containerWidth = barContainer.rect.width;
        float fillWidth = containerWidth * displayedProgress;

        Vector2 fillSize = barFill.sizeDelta;
        fillSize.x = fillWidth;
        barFill.sizeDelta = fillSize;

        if (thiefIcon != null)
        {
            Vector2 thiefPos = thiefIcon.anchoredPosition;
            thiefPos.x = fillWidth - (containerWidth * 0.5f) + thiefOffset;
            thiefIcon.anchoredPosition = thiefPos;
        }
    }

    private void AnimateLoadingText()
    {
        if (loadingText == null) return;

        int dotCount = (int)(Time.unscaledTime * 2f) % 4;
        loadingText.text = "Loading" + new string('.', dotCount);
    }
}