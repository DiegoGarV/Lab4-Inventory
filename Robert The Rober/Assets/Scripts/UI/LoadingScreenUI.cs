using TMPro;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Animator barAnimator;
    [SerializeField] private TMP_Text loadingText;

    [Header("Animator Parameter")]
    [SerializeField] private string loadingParameterName = "LoadingNormalized";

    [Header("Visual Progress")]
    [SerializeField] private float progressLerpSpeed = 0.8f;
    [SerializeField] private float minimumLoadingDuration = 1.5f;

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
            progressLerpSpeed * Time.unscaledDeltaTime
        );

        if (allowFinish && realProgress >= 1f)
        {
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                1f,
                (progressLerpSpeed * 1.5f) * Time.unscaledDeltaTime
            );
        }

        UpdateBarVisuals();
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
        if (barAnimator != null)
        {
            barAnimator.SetFloat(loadingParameterName, displayedProgress);
        }
    }

    private void AnimateLoadingText()
    {
        if (loadingText == null) return;

        int dotCount = (int)(Time.unscaledTime * 2f) % 4;
        loadingText.text = "Loading" + new string('.', dotCount);
    }
}