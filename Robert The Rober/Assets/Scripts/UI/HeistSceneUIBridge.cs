using UnityEngine;

public class HeistSceneUIBridge : MonoBehaviour
{
    public void ResumeGame()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResumeGame();
        }
    }

    public void RestartGame()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RestartGame();
        }
    }

    public void CloseGame()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseGame();
        }
    }

    public void SaveAndQuit()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SafeCloseGame();
        }
    }

    public void GoToStore()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.GoToStore();
        }
    }
}