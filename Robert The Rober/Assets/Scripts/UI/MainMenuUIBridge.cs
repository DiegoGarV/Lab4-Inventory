using UnityEngine;

public class MainMenuUIBridge : MonoBehaviour
{
    public void NewGame()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.NewGame();
    }

    public void LoadGame()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.LoadGame();
    }

    public void OpenSettings()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OpenSettings();
    }
}