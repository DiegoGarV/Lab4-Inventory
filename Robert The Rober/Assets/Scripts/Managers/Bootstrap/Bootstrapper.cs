using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }
}
