using UnityEngine;

public class StoreDoorController : MonoBehaviour
{
    public void Interact()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowReturnToTownPrompt();
        }
    }
}
