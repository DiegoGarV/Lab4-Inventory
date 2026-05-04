using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private string loadingSceneName = "LoadingScreenScene";

    private LoadingScreenUI loadingScreenUI;
    private bool isTransitioning = false;

    public bool IsTransitioning => isTransitioning;

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

    public void LoadSceneWithLoadingScreen(string targetSceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadSceneRoutine(targetSceneName));
    }

    private IEnumerator LoadSceneRoutine(string targetSceneName)
    {
        isTransitioning = true;

        if (!SceneManager.GetSceneByName(loadingSceneName).isLoaded)
        {
            AsyncOperation loadLoadingScene = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

            while (!loadLoadingScene.isDone)
            {
                yield return null;
            }
        }

        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
        SceneManager.SetActiveScene(loadingScene);

        loadingScreenUI = FindFirstObjectByType<LoadingScreenUI>(FindObjectsInactive.Include);
        if (loadingScreenUI != null)
        {
            loadingScreenUI.ResetScreen();
        }

        yield return null;

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
        sceneLoadOperation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (sceneLoadOperation.progress < 0.9f)
        {
            float realProgress = Mathf.Clamp01(sceneLoadOperation.progress / 0.9f);

            if (loadingScreenUI != null)
                loadingScreenUI.SetRealProgress(realProgress);

            yield return null;
        }

        if (loadingScreenUI != null)
            loadingScreenUI.MarkLoadComplete();

        while (loadingScreenUI != null && !loadingScreenUI.IsVisuallyComplete)
            yield return null;

        sceneLoadOperation.allowSceneActivation = true;

        while (!sceneLoadOperation.isDone)
            yield return null;

        isTransitioning = false;
    }
}