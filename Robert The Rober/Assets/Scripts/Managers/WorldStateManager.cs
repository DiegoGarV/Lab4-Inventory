using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
