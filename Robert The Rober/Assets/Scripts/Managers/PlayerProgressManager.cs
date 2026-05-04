using UnityEngine;

public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager Instance;

    private int lastHeistTotal = 0;

    public int LastHeistTotal => lastHeistTotal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLastHeistTotal(int value)
    {
        lastHeistTotal = value;
    }
}