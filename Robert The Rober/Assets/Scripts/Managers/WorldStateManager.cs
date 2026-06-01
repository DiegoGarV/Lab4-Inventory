using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    [SerializeField] private bool isPlayerInsideHouse = false;

    public bool IsPlayerInsideHouse => isPlayerInsideHouse;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TogglePlayerInsideHouse()
    {
        isPlayerInsideHouse = !isPlayerInsideHouse;
    }
}
