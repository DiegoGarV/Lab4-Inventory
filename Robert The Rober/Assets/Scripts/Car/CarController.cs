using UnityEngine;

public class CarController : MonoBehaviour
{
    public void Interact()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowRunawayScreen();
        }
        else
        {
            Debug.LogWarning("UIManager.Instance es null.");
        }
    }
}