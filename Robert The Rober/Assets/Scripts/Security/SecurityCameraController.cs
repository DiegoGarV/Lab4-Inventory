using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject visionVisual;

    private bool isPowered = true;

    public bool IsPowered => isPowered;

    private void Start()
    {
        UpdateVisionVisual();
    }

    public void SetPowered(bool value)
    {
        isPowered = value;
        UpdateVisionVisual();
    }

    private void UpdateVisionVisual()
    {
        if (visionVisual != null)
            visionVisual.SetActive(isPowered);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPowered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCaughtScreen();
        }
        else
        {
            Debug.Log($"Cámara {name}: jugador detectado.");
        }
    }
}