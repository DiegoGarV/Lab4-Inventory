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

    public void CloseBuyItemPrompt()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseBuyItemPrompt();
        }
    }

    public void BuyCurrentStoreItem()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.BuyCurrentStoreItem();
    }

    public void ResumeGame()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResumeGame();
        }
    }

    public void SaveAndQuit()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SafeCloseGame();
        }
    }
}