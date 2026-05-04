using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (Object.FindFirstObjectByType<Bootstrapper>() != null)
            return;

        SceneManager.LoadScene("BootstrapScene", LoadSceneMode.Additive);
    }
}
