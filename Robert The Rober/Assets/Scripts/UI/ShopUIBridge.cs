using UnityEngine;

public class ShopUIBridge : MonoBehaviour
{
    public void ConfirmReturnToTown()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ConfirmReturnToTown();
        }
    }

    public void CancelReturnToTown()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CancelReturnToTown();
        }
    }
}