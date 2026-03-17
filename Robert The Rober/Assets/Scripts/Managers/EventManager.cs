using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public event Action OnMoneyCollected;

    public static EventManager Instance;

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

    public void MoneyCollected()
    {
        OnMoneyCollected?.Invoke();
    }
}
